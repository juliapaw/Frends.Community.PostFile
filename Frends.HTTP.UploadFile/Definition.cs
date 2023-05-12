using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.HTTP.UploadFile
{
    /// <summary>
    /// Represents the HTTP method to be used with the request.
    /// </summary>
    public enum Method
    {
        /// <summary>
        /// The HTTP POST method is used to submit an entity to the specified resource, often causing a change in state or side effects on the server.
        /// </summary>
        POST,
        /// <summary>
        /// The HTTP PUT method is used to replace or update a current resource with new content.
        /// </summary>
        PUT
    }

    /// <summary>
    /// Represents the authentication method to be used with the request.
    /// </summary>
    public enum Authentication
    {
        /// <summary>
        /// No authentication is used.
        /// </summary>
        None,

        /// <summary>
        /// Basic authentication is used, where the username and password are sent in plain text.
        /// </summary>
        Basic,

        /// <summary>
        /// Windows authentication is used, where the user's Windows login credentials are used to authenticate the request.
        /// </summary>
        WindowsAuthentication,

        /// <summary>
        /// Windows integrated security is used, where the current Windows user's credentials are used to authenticate the request.
        /// </summary>
        WindowsIntegratedSecurity,

        /// <summary>
        /// OAuth token-based authentication is used, where a token is obtained from an authentication server and used to authenticate the request.
        /// </summary>
        OAuth,

        /// <summary>
        /// Client certificate-based authentication is used, where a client certificate is used to authenticate the request.
        /// </summary>
        ClientCertificate
    }

    /// <summary>
    /// Represents an HTTP header, which consists of a name-value pair.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// The name of the header.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the header.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents the input data for the HTTP file upload operation.
    /// </summary>
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

    /// <summary>
    /// Options for the HTTP request.
    /// </summary>
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

        /// <summary>
        /// Password for the user.
        /// </summary>
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

    /// <summary>
    /// Represents the response received from the HTTP server after sending a request.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// The body of the response.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The headers of the response, as a dictionary of key-value pairs.
        /// </summary>
       public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// The status code of the response, as an integer value.
        /// </summary>
        public int StatusCode { get; set; }
    }
}