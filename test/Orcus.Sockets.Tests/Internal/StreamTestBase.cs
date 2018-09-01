using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Orcus.Sockets.Tests.Internal
{
    public abstract class StreamTestBase
    {
        protected const int DataSize = 250;

        public static readonly TheoryData<IEnumerable<byte[]>> TestData;

        static StreamTestBase()
        {
            TestData = new TheoryData<IEnumerable<byte[]>>
            {
                //one buffer, read exactly the buffer size
                new[] {CreateBuffer(DataSize)},

                //one buffer that is larger than the read size
                new[] {CreateBuffer(DataSize * 2)},

                //one buffer that is smaller than the read size
                new[] {CreateBuffer(DataSize / 6)},

                //two buffers that are smaller than the read size but have in total the read size
                new[] {CreateBuffer(DataSize / 2), CreateBuffer(DataSize / 2)},

                //two buffers that are smaller than the read size but dont have in total the read size
                new[] {CreateBuffer(DataSize / 3), CreateBuffer(DataSize / 3)},

                //two buffers that are exactly the read size
                new[] {CreateBuffer(DataSize), CreateBuffer(DataSize)},

                //two buffers that are larger than the read size
                new[] {CreateBuffer(DataSize * 2), CreateBuffer(DataSize * 2)},

                //two buffers, one slightly smaller than read size, one much larger
                new[] {CreateBuffer(DataSize - 50), CreateBuffer(DataSize * 3)},

                //two buffers, one slightly smaller than read size, one much larger
                new[] {CreateBuffer(DataSize * 3), CreateBuffer(DataSize - 50)},

                //three buffers, all smaller than the read size and smaller than total size
                new[] {CreateBuffer(DataSize / 6), CreateBuffer(DataSize / 6), CreateBuffer(DataSize / 6)}
            };
        }

        protected void AssertZero(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++) Assert.Equal(0, buffer[offset + i]);
        }

        protected static byte[] CreateBuffer(int length)
        {
            var buffer = new byte[length];
            StaticRandom.NextBytes(buffer);
            return buffer;
        }

        protected static byte[] Merge(IReadOnlyList<ArraySegment<byte>> buffers)
        {
            var buffer = new byte[buffers.Sum(x => x.Count)];
            var pos = 0;

            for (var i = 0; i < buffers.Count; i++)
            {
                var block = buffers[i];
                Buffer.BlockCopy(block.Array, block.Offset, buffer, pos, block.Count);
                pos += block.Count;
            }

            return buffer;
        }
    }
}