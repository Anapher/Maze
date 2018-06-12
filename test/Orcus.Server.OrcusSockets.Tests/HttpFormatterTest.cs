using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Orcus.Modules.Api.Request;
using Orcus.Server.OrcusSockets.Internal.Http;
using Xunit;

namespace Orcus.Server.OrcusSockets.Tests
{
    public class HttpFormatterTest
    {
        private class TestRequest : OrcusRequest
        {
            public override string Method { get; set; }
            public override PathString Path { get; set; }
            public override QueryString QueryString { get; set; }
            public override IQueryCollection Query { get; set; }
            public override IHeaderDictionary Headers { get; set; }
            public override long? ContentLength { get; set; }
            public override string ContentType { get; set; }
            public override Stream Body { get; set; }
        }

        [Theory]
        [InlineData("GET", "/wiki/SpecialCats", null, "/wiki/SpecialCats")]
        [InlineData("POST", "/wiki/SpecialCats", "?hello=world", "/wiki/SpecialCats?hello=world")]
        [InlineData("IDK", "/", "?hello=world", "/?hello=world")]
        [InlineData("XD", "/a very long path with whitespaces", "?hello=world", "/a%20very%20long%20path%20with%20whitespaces?hello=world")]
        public void TestFormatRequest(string method, string path, string query, string expectedPathAndQuery)
        {
            var request = new TestRequest
            {
                Method = method,
                Path = path,
                QueryString = query == null ? QueryString.Empty : new QueryString(query),
                Headers = new HeaderDictionary {{"Content-Type", "yomama"}}
            };

            FormatRequestAndValidate(request, $"{method} {expectedPathAndQuery}\r\nContent-Type: yomama\r\n\r\n");
        }

        [Fact]
        public void TestFormatRequestMultipleHeaders()
        {
            var request = new TestRequest
            {
                Method = HttpMethods.Post,
                Path = "/helloWorld",
                QueryString = QueryString.Empty,
                Headers = new HeaderDictionary
                {
                    {"Content-Type", "yomama"},
                    {"Host", "localhost"},
                    {"Content-Length", "18921"}
                }
            };

            FormatRequestAndValidate(request, "POST /helloWorld\r\nContent-Type: yomama\r\nHost: localhost\r\nContent-Length: 18921\r\n\r\n");
        }

        [Fact]
        public void TestFormatRequestWithHeaderWithMultipleValues()
        {
            var request = new TestRequest
            {
                Method = HttpMethods.Post,
                Path = "/helloWorld",
                QueryString = QueryString.Empty,
                Headers = new HeaderDictionary
                {
                    {"Content-Type", new StringValues(new[] {"json", "xml", "xaml"})}
                }
            };

            FormatRequestAndValidate(request, "POST /helloWorld\r\nContent-Type: json,xml,xaml\r\n\r\n");
        }

        [Fact]
        public void TestFormatRequestWithHeaderWithMultipleValuesWithCommas()
        {
            var request = new TestRequest
            {
                Method = HttpMethods.Post,
                Path = "/helloWorld",
                QueryString = QueryString.Empty,
                Headers = new HeaderDictionary
                {
                    {"Content-Type", new StringValues(new[] {"json,net", "xml", "xaml"})}
                }
            };

            FormatRequestAndValidate(request, "POST /helloWorld\r\nContent-Type: \"json,net\",xml,xaml\r\n\r\n");
        }

        private static void FormatRequestAndValidate(OrcusRequest request, string expected)
        {
            var buffer = new byte[8192];
            var result = HttpFormatter.FormatRequest(request, buffer);
            var decodedString = Encoding.UTF8.GetString(buffer, 0, result);

            Assert.Equal(expected, decodedString);
        }
    }
}