using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpMock;
using NUnit.Framework;

namespace Frends.Community.PostFile.Tests
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
        private static IEnumerable<Func<Input, Options, CancellationToken, Task<object>>> TestCases
        {
            get
            {
                yield return PostFileTask.PostFile;
            }
        }

        [Test]
        public async Task RequestShouldSetEncodingWithContentTypeCharsetIgnoringCase()
        {
            var fileLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\Test_files\test_file.txt");
            var codePageName = "iso-8859-1";
            var utf8ByteArray = File.ReadAllBytes(fileLocation);
            var expectedContentType = $"text/plain; charset={codePageName}";

            _stubHttp.Stub(x => x.Post("/endpoint"))
                .AsContentType($"text/plain; charset={codePageName}")
                .Return("foo åäö")
                .OK();

            var contentType = new Header { Name = "cONTENT-tYpE", Value = expectedContentType };
            var input = new Input { Method = Method.POST, Url = "http://localhost:9191/endpoint", Headers = new Header[1] { contentType }, FileLocation = fileLocation };
            var options = new Options { ConnectionTimeoutSeconds = 60 };
            var result = (Response)await PostFileTask.PostFile(input, options, CancellationToken.None);
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
