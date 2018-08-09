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

        public ArrayPoolMemoryStream(ArraySegment<byte> buffer) : base(buffer.Array, buffer.Offset, buffer.Count, false)
        {
            _buffer = buffer;
        }

        protected override void Dispose(bool disposing)
        {
            ArrayPool<byte>.Shared.Return(_buffer.Array);
        }
    }
}