using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RequestTransmitter.Client.Utilities;
using Xunit;

namespace RequestTransmitter.Client.Tests.Utilities
{
    public class HttpSerializerTests
    {
        [Fact]
        public async Task TestSerializeHttpRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/test/help?asd=asd");
            request.Content = new StringContent("Test");

            using (var memoryStream = new MemoryStream())
            {
                await HttpSerializer.Format(request, memoryStream);

                memoryStream.Position = 0;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    var s = streamReader.ReadToEnd();
                    const string expected = @"POST /test/help?asd=asd

Test";

                    Assert.Equal(expected, s);
                }
            }
        }

        [Fact]
        public async Task TestSerializeHttpRequestWithHeaders()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.de/test/help?asd=asd");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "wtf");
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf8"));

            request.Content = new ByteArrayContent(new byte[] { 1, 2, 3, 4, 5, 5, 44, 54, 23, 34 });
            request.Content.Headers.ContentLength = 100;
            request.Content.Headers.ContentEncoding.Add("ascii");

            using (var memoryStream = new MemoryStream())
            {
                await HttpSerializer.Format(request, memoryStream);

                memoryStream.Position = 0;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    var s = streamReader.ReadToEnd();
                    const string expected = @"GET https://www.google.de/test/help?asd=asd
Authorization: Bearer wtf
Accept-Encoding: utf8
Content-Length: 100
Content-Encoding: ascii

,6""";

                    Assert.Equal(expected, s);
                }
            }
        }

        [Fact]
        public async Task TestDecodeHttpRequest()
        {
            const string requestString = @"POST /test/help?asd=asd

Test";

            using (var request = await HttpSerializer.Decode(new MemoryStream(Encoding.UTF8.GetBytes(requestString))))
            {
                Assert.Equal(HttpMethod.Post.Method, request.Method.Method);
                Assert.Equal(new Uri("/test/help?asd=asd", UriKind.Relative), request.RequestUri);

                var contentString = await request.Content.ReadAsStringAsync();
                Assert.Equal("Test", contentString);
            }
        }

        [Fact]
        public async Task TestDecodeHttpRequestWithHeaders()
        {
            const string requestString = @"GET https://www.google.de/test/help?asd=asd
Authorization: Bearer wtf
Accept-Encoding: utf8
Content-Length: 100
Content-Encoding: ascii

,6""";

            using (var request = await HttpSerializer.Decode(new MemoryStream(Encoding.UTF8.GetBytes(requestString))))
            {
                Assert.Equal(HttpMethod.Get.Method, request.Method.Method);
                Assert.Equal(new Uri("https://www.google.de/test/help?asd=asd", UriKind.Absolute), request.RequestUri);

                Assert.Equal("Bearer wtf", request.Headers.Authorization.ToString());
                Assert.Contains("utf8", request.Headers.AcceptEncoding.Select(x => x.Value));
                Assert.Equal(100, request.Content.Headers.ContentLength);
                Assert.Contains("ascii", request.Content.Headers.ContentEncoding);

                var contentArray = await request.Content.ReadAsByteArrayAsync();
                Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 5, 44, 54, 23, 34 }, contentArray);
            }
        }
    }
}
