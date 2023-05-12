using HttpMock;
using HttpMock.Verify.NUnit;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.HTTP.UploadFile.Tests
{
    [TestFixture]
    class TestClass
    {
        [TestFixture]
        public class UnitTest
        {
            private IHttpServer _stubHttp;

            [SetUp]
            public void Setup()
            {
                _stubHttp = HttpMockRepository.At("http://localhost:9191");
            }

            [Test]
            public async Task RequestShouldSetEncodingWithContentTypeCharsetIgnoringCase()
            {
                var fileLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../..", "Test_files", "test_file.txt"));
                var codePageName = "iso-8859-1";
                var utf8ByteArray = File.ReadAllBytes(fileLocation);
                var expectedContentType = $"text/plain; charset={codePageName}";

                _stubHttp.Stub(x => x.Post("/endpoint"))
                    .AsContentType($"text/plain; charset={codePageName}")
                    .Return("foo едц")
                    .OK();

                var contentType = new Header { Name = "cONTENT-tYpE", Value = expectedContentType };
                var input = new Input { Method = Method.POST, Url = "http://localhost:9191/endpoint", Headers = new Header[1] { contentType }, FileLocation = fileLocation };
                var options = new Options { ConnectionTimeoutSeconds = 60 };
                var result = (Response)await UploadFileTask.UploadFile(input, options, CancellationToken.None);
                var request = _stubHttp.AssertWasCalled(called => called.Post("/endpoint")).LastRequest();
                var requestHead = request.RequestHead;
                var requestBodyByteArray = Encoding.GetEncoding(codePageName).GetBytes(request.Body);
                var requestContentType = requestHead.Headers["cONTENT-tYpE"];

                //Casing should not affect setting header.
                Assert.That(requestContentType, Is.EqualTo(expectedContentType));
                Assert.That(requestBodyByteArray, Is.EqualTo(utf8ByteArray));
            }
        }
    }
}