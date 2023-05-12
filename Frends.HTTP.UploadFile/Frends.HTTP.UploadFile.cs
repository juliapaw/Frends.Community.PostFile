using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.HTTP.UploadFile
{
    /// <summary>
    /// Represents a task that posts a file to a web API endpoint.
    /// </summary>
    public static class UploadFileTask
    {
        /// <summary>
        /// Send file using StreamContent
        /// </summary>
        /// <param name="input">Input parameters</param>
        /// <param name="options">Optional parameters with default values</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel the upload operation.</param>
        /// <returns>Object with the following properties: JToken Body. Dictionary(string,string) Headers. int StatusCode</returns>
        /// public static bool Delete([PropertyTab] string fileName, [PropertyTab] OptionsClass options)
        public static async Task<object> UploadFile([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.SetHandleSettingsBasedOnOptions(options);

                using (var httpClient = new HttpClient(handler))
                {
                    var responseMessage = await GetHttpRequestResponseAsync(httpClient, input, options, cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    string body = string.Empty;
                    IEnumerable<KeyValuePair<string, IEnumerable<string>>> contentHeaders = new Dictionary<string, IEnumerable<string>>();

                    if (responseMessage.Content != null)
                    {
                        body = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                        contentHeaders = responseMessage.Content.Headers;
                    }
                    var response = new Response
                    {
                        Body = body,
                        StatusCode = (int)responseMessage.StatusCode,
                        Headers = GetResponseHeaderDictionary((IEnumerable<KeyValuePair<string, IEnumerable<string>>>)responseMessage.Headers ?? new Dictionary<string, IEnumerable<string>>(), contentHeaders)
                    };

                    if (!responseMessage.IsSuccessStatusCode && options.ThrowExceptionOnErrorResponse)
                    {
                        throw new WebException($"Request to '{input.Url}' failed with status code {(int)responseMessage.StatusCode}. Response body: {response.Body}");
                    }

                    return response;
                }
            }
        }

        //Combine response- and responsecontent header to one dictionary
        private static Dictionary<string, string> GetResponseHeaderDictionary(IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseMessageHeaders, IEnumerable<KeyValuePair<string, IEnumerable<string>>> contentHeaders)
        {
            var responseHeaders = responseMessageHeaders.ToDictionary(h => h.Key, h => string.Join(";", h.Value));
            var allHeaders = contentHeaders.ToDictionary(h => h.Key, h => string.Join(";", h.Value));
            responseHeaders.ToList().ForEach(x => allHeaders[x.Key] = x.Value);
            return allHeaders;
        }

        private static async Task<HttpResponseMessage> GetHttpRequestResponseAsync(HttpClient httpClient, Input input, Options options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (options.Authentication == Authentication.Basic || options.Authentication == Authentication.OAuth)
            {
                switch (options.Authentication)
                {
                    case Authentication.Basic:
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{options.Username}:{options.Password}")));
                        break;
                    case Authentication.OAuth:
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                            options.Token);
                        break;
                }
            }

            //Do not automtically set expect 100-continue response header
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(options.ConnectionTimeoutSeconds));

            //Ignore case for headers and key comparison
            var headerDict = input.Headers.ToDictionary(key => key.Name, value => value.Value, StringComparer.InvariantCultureIgnoreCase);

            using (MemoryStream reader = new MemoryStream(File.ReadAllBytes(input.FileLocation)))
            using (HttpContent content = new StreamContent(reader))
            {
                //Clear default headers
                content.Headers.Clear();
                foreach (var header in headerDict)
                {
                    var requestHeaderAddedSuccessfully = httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    if (!requestHeaderAddedSuccessfully)
                    {
                        //Could not add to request headers try to add to content headers
                        var contentHeaderAddedSuccessfully = content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        if (!contentHeaderAddedSuccessfully)
                        {
                            Trace.TraceWarning($"Could not add header {header.Key}:{header.Value}");
                        }
                    }
               
                }

                var request = new HttpRequestMessage(new HttpMethod(input.Method.ToString()), new Uri(input.Url))
                {
                    Content = content
                };

                var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (options.AllowInvalidResponseContentTypeCharSet)
                {
                    response.Content.Headers.ContentType.CharSet = null;
                }
                return response;
            }
        }      
    }

    /// <summary>
    /// Provides extension methods for various types, allowing for additional functionality to be added to existing types.
    /// </summary>
    public static class Extensions
    {
        internal static void SetHandleSettingsBasedOnOptions(this HttpClientHandler handler, Options options)
        {
            switch (options.Authentication)
            {
                case Authentication.WindowsIntegratedSecurity:
                    handler.UseDefaultCredentials = true;
                    break;
                case Authentication.WindowsAuthentication:
                    var domainAndUserName = options.Username.Split('\\');
                    if (domainAndUserName.Length != 2)
                    {
                        throw new ArgumentException($@"Username needs to be 'domain\username' now it was '{options.Username}'");
                    }
                    handler.Credentials = new NetworkCredential(domainAndUserName[1], options.Password, domainAndUserName[0]);
                    break;
                case Authentication.ClientCertificate:
                    handler.ClientCertificates.Add(GetCertificate(options.CertificateThumbprint));
                    break;
            }

            handler.AllowAutoRedirect = options.FollowRedirects;

            if (options.AllowInvalidCertificate)
            {
                handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
            }
        }

        internal static X509Certificate2 GetCertificate(string thumbprint)
        {
            thumbprint = Regex.Replace(thumbprint, @"[^\da-zA-z]", string.Empty).ToUpper();
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var signingCert = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (signingCert.Count == 0)
                {
                    throw new FileNotFoundException($"Certificate with thumbprint: '{thumbprint}' not found in current user cert store.");
                }

                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}