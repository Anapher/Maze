using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Orcus.Modules.Api.Response;
using Orcus.Sockets.Tests.Internal;
using Xunit;

namespace Orcus.Sockets.Tests.OrcusServerTests
{
    public class TestRespondingDifferentSizesWithCompression : StreamTestBase
    {
        private class OrcusServerDataSendTest : OrcusServerTestBase
        {
            private readonly IReadOnlyList<byte[]> _packages;

            protected override int PackageSize => DataSize;
            protected override int MaxHeaderSize => DataSize - 50;

            public OrcusServerDataSendTest(IEnumerable<byte[]> packages)
            {
                _packages = packages.ToList();
            }

            protected override HttpRequestMessage GetRequest()
            {
                var request = base.GetRequest();
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                return request;
            }

            protected override async Task WriteResponse(OrcusResponse response)
            {
                foreach (var package in _packages)
                {
                    await response.Body.WriteAsync(package, 0, package.Length);
                }
            }

            protected override async Task AssertReceivedResponse(OrcusResponse response, HttpResponseMessage responseMessage)
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
            var senderTest = new OrcusServerDataSendTest(packages);
            await senderTest.ExecuteTest();
        }
    }
}