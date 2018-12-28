using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Maze.Modules.Api.Response;
using Maze.Sockets.Tests.Internal;
using Xunit;

namespace Maze.Sockets.Tests.MazeServerTests
{
    public class TestRespondingDifferentSizesWithCompression : StreamTestBase
    {
        private class MazeServerDataSendTest : MazeServerTestBase
        {
            private readonly IReadOnlyList<byte[]> _packages;

            protected override int PackageSize => DataSize;
            protected override int MaxHeaderSize => DataSize - 50;

            public MazeServerDataSendTest(IEnumerable<byte[]> packages)
            {
                _packages = packages.ToList();
            }

            protected override HttpRequestMessage GetRequest()
            {
                var request = base.GetRequest();
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                return request;
            }

            protected override async Task WriteResponse(MazeResponse response)
            {
                foreach (var package in _packages)
                {
                    await response.Body.WriteAsync(package, 0, package.Length);
                }
            }

            protected override async Task AssertReceivedResponse(MazeResponse response, HttpResponseMessage responseMessage)
            {
                var expected = Merge(_packages.Select(x => new ArraySegment<byte>(x)).ToList());
                byte[] actual;

                if (responseMessage.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    using (var result = new MemoryStream())
                    {
                        using (var gzipStream = new GZipStream(await responseMessage.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
                        {
                            await gzipStream.CopyToAsync(result);
                        }

                        actual = result.ToArray();
                    }
                }
                else
                {
                    using (var result = new MemoryStream())
                    {
                        await responseMessage.Content.CopyToAsync(result);
                        actual = result.ToArray();
                    }
                }

                Assert.Equal(expected.Length, actual.Length);
                Assert.Equal(expected, actual);
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task TestWriteData(IEnumerable<byte[]> packages)
        {
            var senderTest = new MazeServerDataSendTest(packages);
            await senderTest.ExecuteTest();
        }
    }
}