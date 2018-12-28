using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Maze.Modules.Api.Response;
using Xunit;

namespace Maze.Sockets.Tests.MazeServerTests
{
    public class TestAutomaticResponseCompression : MazeServerTestBase
    {
        private readonly byte[] _testData;

        protected override int PackageSize { get; } = 101;
        protected override int MaxHeaderSize { get; } = 100;

        public TestAutomaticResponseCompression()
        {
            _testData = new byte[PackageSize * 2];
            StaticRandom.NextBytes(_testData);
        }

        protected override HttpRequestMessage GetRequest()
        {
            var request = base.GetRequest();
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            return request;
        }

        protected override Task WriteResponse(MazeResponse response)
        {
            return response.Body.WriteAsync(_testData, 0, _testData.Length);
        }

        protected override async Task AssertReceivedResponse(MazeResponse response, HttpResponseMessage responseMessage)
        {
            Assert.Contains(responseMessage.Content.Headers.ContentEncoding, s => s == "gzip");

            using (var receiveStream = new MemoryStream())
            {
                await responseMessage.Content.CopyToAsync(receiveStream);
                receiveStream.Position = 0;

                using (var result = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(receiveStream, CompressionMode.Decompress))
                    {
                        await gzipStream.CopyToAsync(result);
                    }

                    var actual = result.ToArray();
                    Assert.Equal(_testData.Length, actual.Length);
                    Assert.Equal(_testData, actual);
                }
            }
        }
    }
}