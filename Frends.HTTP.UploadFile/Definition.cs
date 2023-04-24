#pragma warning disable 1591
using Frends.HTTP.UploadFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.HTTP.UploadFile
{
	public enum Method
    {
        POST, PUT
    }

    public enum Authentication
    {
        None, Basic, WindowsAuthentication, WindowsIntegratedSecurity, OAuth, ClientCertificate
    }

    public class Header
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Input
    {
        /// <summary>
        /// The HTTP Method to be used with the request.
        /// </summary>
        public Method Method { get; set; }

        /// <summary>
        /// The URL with protocol and path. You can include query parameters directly in the url.
        /// </summary>
        [DefaultValue("https://example.org/path/to")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Url { get; set; }

        /// <summary>
        /// The file location to be posted
        /// </summary>
        public string FileLocation { get; set; }

        /// <summary>
        /// List of HTTP headers to be added to the request.
        /// </summary>
        public Header[] Headers { get; set; }
    }

    public class Options
    {
        /// <summary>
        /// Method of authenticating request
        /// </summary>
        public Authentication Authentication { get; set; }

        /// <summary>
        /// If WindowsAuthentication is selected you should use domain\username
        /// </summary>
        [UIHint(nameof(UploadFile.Authentication), "", Authentication.WindowsAuthentication, Authentication.Basic)]
        public string Username { get; set; }

        [PasswordPropertyText]
        [UIHint(nameof(UploadFile.Authentication), "", Authentication.WindowsAuthentication, Authentication.Basic)]
        public string Password { get; set; }

        /// <summary>
        /// Bearer token to be used for request. Token will be added as Authorization header.
        /// </summary>
        [PasswordPropertyText]
        [UIHint(nameof(UploadFile.Authentication), "", Authentication.OAuth)]
        public string Token { get; set; }

        /// <summary>
        /// Thumbprint for using client certificate authentication.
        /// </summary>
        [UIHint(nameof(UploadFile.Authentication), "", Authentication.ClientCertificate)]
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// Timeout in seconds to be used for the connection and operation.
        /// </summary>
        [DefaultValue(30)]
        public int ConnectionTimeoutSeconds { get; set; }

        /// <summary>
        /// If FollowRedirects is set to false, all responses with an HTTP status code from 300 to 399 is returned to the application.
        /// </summary>
        [DefaultValue(true)]
        public bool FollowRedirects { get; set; }

        /// <summary>
        /// Do not throw an exception on certificate error.
        /// </summary>
        public bool AllowInvalidCertificate { get; set; }

        /// <summary>
        /// Some Api's return faulty content-type charset header. This setting overrides the returned charset.
        /// </summary>
        public bool AllowInvalidResponseContentTypeCharSet { get; set; }
        /// <summary>
        /// Throw exception if return code of request is not successfull
        /// </summary>
        public bool ThrowExceptionOnErrorResponse { get; set; }
    }

    public class Response
    {
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int StatusCode { get; set; }
    }
}
