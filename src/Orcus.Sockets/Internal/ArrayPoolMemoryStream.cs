using System;
using System.Buffers;
using System.IO;

namespace Orcus.Sockets.Internal
{
    /// <summary>
    ///     A simple memory stream that returns the buffer to the <see cref="ArrayPool{T}" /> on disposing
    /// </summary>
    public class ArrayPoolMemoryStream : MemoryStream
    {
        private readonly ArraySegment<byte> _buffer;
        private readonly ArrayPool<byte> _pool;

        public ArrayPoolMemoryStream(ArraySegment<byte> buffer, ArrayPool<byte> pool) : base(buffer.Array, buffer.Offset, buffer.Count, false)
        {
            _buffer = buffer;
            _pool = pool;
        }

        protected override void Dispose(bool disposing)
        {
            _pool?.Return(_buffer.Array);
        }
    }
}