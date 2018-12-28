using System;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Primitives;
using Maze.Sockets.Internal.Http;
using Xunit;

namespace Maze.Sockets.Tests
{
    public class HttpFormatterTest
    {
        [Theory]
        [InlineData("GET", "/wiki/SpecialCats", null, "/wiki/SpecialCats")]
        [InlineData("POST", "/wiki/SpecialCats", "?hello=world", "/wiki/SpecialCats?hello=world")]
        [InlineData("IDK", "/", "?hello=world", "/?hello=world")]
        [InlineData("XD", "/a very long path with whitespaces", "?hello=world",
            "/a%20very%20long%20path%20with%20whitespaces?hello=world")]
        public void TestFormatRequest(string method, string path, string query, string expectedPathAndQuery)
        {
            var uriBuilder = new UriBuilder {Path = path, Query = query};
            var request = new HttpRequestMessage {Method = new HttpMethod(method), RequestUri = uriBuilder.Uri};
            request.Headers.Add("irgendwas", "yomama");

            FormatRequestAndValidate(request, $"{method} {expectedPathAndQuery}\r\nirgendwas: yomama\r\n\r\n");
        }

        private static void FormatRequestAndValidate(HttpRequestMessage request, string expected)
        {
            var buffer = new byte[8192];
            var result = HttpFormatter.FormatRequest(request, buffer);
            var decodedString = Encoding.UTF8.GetString(buffer, 0, result);

            Assert.Equal(expected, decodedString);
        }

        [Fact]
        public void TestFormatRequestMultipleHeaders()
        {
            var uriBuilder = new UriBuilder {Path = "hello/world"};
            var request = new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = uriBuilder.Uri};

            request.Headers.Add("Host", "localhost");
            request.Headers.Add("WTF", "ASD");
            request.Headers.Add("Content-Hey", "18921");

            FormatRequestAndValidate(request,
                "POST /hello/world\r\nHost: localhost\r\nWTF: ASD\r\nContent-Hey: 18921\r\n\r\n");
        }

        [Fact]
        public void TestFormatRequestWithHeaderWithMultipleValues()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost/helloWorld")
            };
            request.Headers.Add("Test", new StringValues(new[] {"json", "xml", "xaml"}).ToArray());

            FormatRequestAndValidate(request, "POST /helloWorld\r\nTest: json,xml,xaml\r\n\r\n");
        }

        [Fact]
        public void TestFormatRequestWithHeaderWithMultipleValuesWithCommas()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost/helloWorld")
            };
            request.Headers.Add("Test", new StringValues(new[] {"json,net", "xml", "xaml"}).ToArray());

            FormatRequestAndValidate(request, "POST /helloWorld\r\nTest: \"json,net\",xml,xaml\r\n\r\n");
        }
    }
}