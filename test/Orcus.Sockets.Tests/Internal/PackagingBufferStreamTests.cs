using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orcus.Sockets.Internal;
using Xunit;

namespace Orcus.Sockets.Tests.Internal
{
    public class PackagingBufferStreamTests : StreamTestBase
    {
        private readonly List<byte[]> _pushedPackages = new List<byte[]>();
        private readonly PackagingBufferStream _stream;

        public PackagingBufferStreamTests()
        {
            _stream = new PackagingBufferStream(SendPackageDelegate, DataSize);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task TestWriteData(IEnumerable<byte[]> packages)
        {
            var buffers = packages.ToList();
            foreach (var package in buffers)
            {
                await _stream.WriteAsync(package, 0, package.Length);
            }

            foreach (var pushedPackage in _pushedPackages)
            {
                Assert.Equal(DataSize, pushedPackage.Length);
            }

            await _stream.FlushAsync();

            var expectedData = Merge(buffers.Select(x => new ArraySegment<byte>(x)).ToList());
            var actualData = Merge(_pushedPackages.Select(x => new ArraySegment<byte>(x)).ToList());

            Assert.Equal(expectedData, actualData);
        }

        private Task SendPackageDelegate(ArraySegment<byte> data)
        {
            if (data.Array == null)
                return Task.CompletedTask;

            var copy = new byte[data.Count];
            Buffer.BlockCopy(data.Array, data.Offset, copy, 0, data.Count);
            _pushedPackages.Add(copy);

            return Task.CompletedTask;
        }
    }
}
