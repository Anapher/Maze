using System;
using System.IO;
using System.Threading.Tasks;
using Maze.Sockets.Internal;
using Xunit;

namespace Maze.Sockets.Tests.Internal
{
    public class DelegatingWriteStreamTests
    {
        private readonly Stream _stream;
        private ArraySegment<byte> _delgatedBuffer;

        public DelegatingWriteStreamTests()
        {
            _stream = new DelegatingWriteStream(SendPackageDelegate);
        }

        private Task SendPackageDelegate(ArraySegment<byte> data)
        {
            _delgatedBuffer = data;
            return Task.CompletedTask;
        }

        [Fact]
        public async Task TestWriteAsynchronous()
        {
            var testBuffer = new byte[125];
            StaticRandom.NextBytes(testBuffer);

            await _stream.WriteAsync(testBuffer, 5, 80);

            Assert.Equal(testBuffer, _delgatedBuffer.Array);
            Assert.Equal(5, _delgatedBuffer.Offset);
            Assert.Equal(80, _delgatedBuffer.Count);
        }

        [Fact]
        public void TestWriteSynchronous()
        {
            var testBuffer = new byte[125];
            StaticRandom.NextBytes(testBuffer);

            _stream.Write(testBuffer, 5, 80);

            Assert.Equal(testBuffer, _delgatedBuffer.Array);
            Assert.Equal(5, _delgatedBuffer.Offset);
            Assert.Equal(80, _delgatedBuffer.Count);
        }
    }
}