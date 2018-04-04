# Frends.Community.PostFile
FRENDS Task to send file using StreamContent

- [Tasks](#tasks)
    - [PostFile](#postfile)
- [License](#license)
- [Contributing](#contributing)

Tasks
=====

## PostFile

The Frends.Community.PostFile task is meant for sending files using HTTP. 

Input:

| Property          | Type                               | Description												| Example                                   |
|-------------------|----------------------------------------|------------------------------------------------------|-------------------------------------------|
| Method            | Enum(Post, Put)					 | Http method of request.									|  `Post`									|
| Url               | string                             | The URL with protocol and path to call.					| `https://foo.example.org/path/to?Id=14`	|
| FileLocation      | string                             | Path to file												| `C:\TestFile.txt`							|
| Headers           | Array{Name: string, Value: string} | List of HTTP headers to be added to the request.			| `Name = Content-Type, Value = text/plain` |

Options:

| Property                         | Type                               | Description                                                         |
|----------------------------------|------------------------------------|---------------------------------------------------------------------|
| Authentication                   | Enum(None, Basic, Windows,WindowsIntegratedSecurity, OAuth, ClientCertificate ) | Different options for authentication for the HTTP request.   |
| Connection Timeout Seconds       | int                                | Timeout in seconds to be used for the connection and operation. Default is 30 seconds. |
| Follow Redirects                 | bool                               | If FollowRedirects is set to false, all responses with an HTTP status code from 300 to 399 is returned to the application. Default is true.|
| Allow Invalid Certificate        | bool                               | Do not throw an exception on certificate error. Setting this to true is discouraged in production. |
| Throw Exception On ErrorResponse | bool                               | Throw a WebException if return code of request is not successful.  |
| AllowInvalidResponseContentTypeCharSet | bool                         | Some Api's return faulty content-type charset header. If set to true this overrides the returned charset. |
| Username                         | string                             | This field is available for Basic- and Windows Authentication. If Windows Authentication is selected Username needs to be of format domain\username. Basic authentication will add a base64 encoded Authorization header consisting of Username and password fields. |
| Password                         | string                             | This field is available for Basic- and Windows Authentication.  |
| Token                            | string                             | Token to be used in an OAuth request. The token will be added as a Authentication header. `Authorization Bearer '{Token}'` |
| CertificateThumbprint            | string                             | This field is used with Client Certificate Authentication. The certificate needs to be found in Cert\CurrentUser\My store on the agent running the process |

Result:

| Property          | Type                      | Description          |
|-------------------|---------------------------|----------------------|
| Body              | JToken                    | Response body        |
| Headers           | Dictionary<string,string> | Response headers     |
| StatusCode        | int                       | Response status code | 


License
=======
This project is licensed under the MIT License - see the LICENSE file for details

Contributing
============
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!
