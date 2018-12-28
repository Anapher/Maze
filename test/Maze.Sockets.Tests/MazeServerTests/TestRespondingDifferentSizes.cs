using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Maze.Modules.Api.Response;
using Maze.Sockets.Tests.Internal;
using Xunit;

namespace Maze.Sockets.Tests.MazeServerTests
{
    public class TestRespondingDifferentSizes : StreamTestBase
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

            protected override async Task WriteResponse(MazeResponse response)
            {
                foreach (var package in _packages)
                {
                    await response.Body.WriteAsync(package, 0, package.Length);
                }
            }

            protected override async Task AssertReceivedResponse(MazeResponse response, HttpResponseMessage responseMessage)
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
            var senderTest = new MazeServerDataSendTest(packages);
            await senderTest.ExecuteTest();
        }
    }
}