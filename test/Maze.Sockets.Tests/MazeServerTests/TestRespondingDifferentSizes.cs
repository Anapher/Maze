using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Modules.Api.Response;
using Orcus.Sockets.Tests.Internal;
using Xunit;

namespace Orcus.Sockets.Tests.OrcusServerTests
{
    public class TestRespondingDifferentSizes : StreamTestBase
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

            protected override async Task WriteResponse(OrcusResponse response)
            {
                foreach (var package in _packages)
                {
                    await response.Body.WriteAsync(package, 0, package.Length);
                }
            }

            protected override async Task AssertReceivedResponse(OrcusResponse response, HttpResponseMessage responseMessage)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await responseMessage.Content.CopyToAsync(memoryStream);

                    var expected = Merge(_packages.Select(x => new ArraySegment<byte>(x)).ToList());
                    var actual = memoryStream.ToArray();
                    Assert.Equal(expected, actual);
                }
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