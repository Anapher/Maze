using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Maze.Sockets.Internal;
using Xunit;

namespace Maze.Sockets.Tests.Internal
{
    public class BufferQueueStreamTests : StreamTestBase
    {
        private const int WaitDelay = 20;

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestSynchronousReadAllBuffersPushedAtBeginningNoSegmentation(IEnumerable<byte[]> data)
        {
            var buffers = data.Select(x => new ArraySegment<byte>(x)).ToArray();
            var expectedResult = Merge(buffers);

            var stream = new BufferQueueStream(null);
            foreach (var buffer in buffers)
                stream.PushBuffer(buffer);

            stream.IsCompleted = true;

            ReadSyncAndAssert(stream, expectedResult, 0);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task TestAsynchronousReadAllBuffersPushedAtBeginningNoSegmentation(IEnumerable<byte[]> data)
        {
            var buffers = data.Select(x => new ArraySegment<byte>(x)).ToArray();
            var expectedResult = Merge(buffers);

            var stream = new BufferQueueStream(null);
            foreach (var buffer in buffers)
                stream.PushBuffer(buffer);

            stream.IsCompleted = true;

            await ReadAsyncAndAssert(stream, expectedResult, 0);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestSynchronousReadAllBuffersPushedAtBeginningWithSegmentation(IEnumerable<byte[]> data)
        {
            var buffers = data.Select(x => new ArraySegment<byte>(x, 6, x.Length - 12)).ToArray();
            var expectedResult = Merge(buffers);

            var stream = new BufferQueueStream(null);
            foreach (var buffer in buffers)
                stream.PushBuffer(buffer);

            stream.IsCompleted = true;

            ReadSyncAndAssert(stream, expectedResult, 0);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task TestAsynchronousReadAllBuffersPushedAtBeginningWithSegmentation(IEnumerable<byte[]> data)
        {
            var buffers = data.Select(x => new ArraySegment<byte>(x, 6, x.Length - 12)).ToArray();
            var expectedResult = Merge(buffers);

            var stream = new BufferQueueStream(null);
            foreach (var buffer in buffers)
                stream.PushBuffer(buffer);

            stream.IsCompleted = true;

            await ReadAsyncAndAssert(stream, expectedResult, 0);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task TestSynchronousReadAsyncPushedBuffers(IEnumerable<byte[]> data)
        {
            var buffers = data.Select(x => new ArraySegment<byte>(x, 6, x.Length - 12)).ToArray();
            var expectedResult = Merge(buffers);

            var stream = new BufferQueueStream(null);
            var readTask = Task.Run(() => ReadSyncAndAssert(stream, expectedResult, 0));

            foreach (var buffer in buffers)
            {
                Assert.False(readTask.IsCompleted);
                await Task.Delay(WaitDelay);
                stream.PushBuffer(buffer);
            }

            await Task.Delay(WaitDelay);
            Assert.False(readTask.IsCompleted);
            stream.IsCompleted = true;

            await Task.Delay(WaitDelay);
            Assert.True(readTask.IsCompleted);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task TestAsynchronousReadAsyncPushedBuffers(IEnumerable<byte[]> data)
        {
            var buffers = data.Select(x => new ArraySegment<byte>(x, 6, x.Length - 12)).ToArray();
            var expectedResult = Merge(buffers);

            var stream = new BufferQueueStream(null);
            var readTask = ReadAsyncAndAssert(stream, expectedResult, 0);

            foreach (var buffer in buffers)
            {
                Assert.False(readTask.IsCompleted);
                await Task.Delay(WaitDelay);
                stream.PushBuffer(buffer);
            }

            await Task.Delay(WaitDelay);
            Assert.False(readTask.IsCompleted);
            stream.IsCompleted = true;

            await Task.Delay(WaitDelay);
            Assert.True(readTask.IsCompleted);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestSynchronousReadInOffsetBasedBuffer(IEnumerable<byte[]> data)
        {
            var buffers = data.Select(x => new ArraySegment<byte>(x)).ToArray();
            var expectedResult = Merge(buffers);

            var stream = new BufferQueueStream(null);
            foreach (var buffer in buffers)
                stream.PushBuffer(buffer);

            stream.IsCompleted = true;

            ReadSyncAndAssert(stream, expectedResult, 50);
        }

        private void ReadSyncAndAssert(Stream stream, byte[] expectedResult, int offset)
        {
            var result = new byte[expectedResult.Length];

            var pos = 0;
            var readBuffer = new byte[DataSize + offset * 2];
            int read;
            while ((read = stream.Read(readBuffer, offset, DataSize)) != 0)
            {
                Assert.True(read <= DataSize);
                AssertZero(readBuffer, 0, offset);
                AssertZero(readBuffer, DataSize + offset, offset);

                Buffer.BlockCopy(readBuffer, offset, result, pos, read);
                pos += read;
            }

            Assert.Equal(expectedResult, result);
        }

        private async Task ReadAsyncAndAssert(Stream stream, byte[] expectedResult, int offset)
        {
            var result = new byte[expectedResult.Length];

            var pos = 0;
            var readBuffer = new byte[DataSize + offset * 2];
            int read;
            while ((read = await stream.ReadAsync(readBuffer, offset, DataSize)) != 0)
            {
                Assert.True(read <= DataSize);
                AssertZero(readBuffer, 0, offset);
                AssertZero(readBuffer, DataSize + offset, offset);

                Buffer.BlockCopy(readBuffer, offset, result, pos, read);
                pos += read;
            }

            Assert.Equal(expectedResult, result);
        }
    }
}