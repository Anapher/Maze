using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Maze.Sockets.Internal.Infrastructure;

namespace Maze.Sockets.Internal
{
    /// <summary>
    ///     A stream that buffers data up to a given size and writes the package all at once
    /// </summary>
    public class BufferingWriteStream : WriteOnlyStream
    {
        private readonly int _packageBufferSize;
        private readonly ArrayPool<byte> _bufferPool;
        private long _length;
        private byte[] _packageBuffer;
        private int _packageBufferOffset;
        private bool _disposed;

        public BufferingWriteStream(Stream stream, int packageBufferSize, ArrayPool<byte> bufferPool)
        {
            InnerStream = stream;
            _packageBufferSize = packageBufferSize;
            _bufferPool = bufferPool;
        }

        public Stream InnerStream { get; private set; }
        public Func<Task> FlushCallback { get; set; }

        public void SetInnerStream(Stream inner)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(WrappingStream));
            }

            InnerStream = inner;
        }

        public override void Flush()
        {
            FlushAsync().Wait();
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (_packageBufferOffset == 0)
            {
                await InnerStream.FlushAsync(cancellationToken);
                return;
            }

            var packageBufferOffset = _packageBufferOffset;
            _packageBufferOffset = 0;
            await InnerStream.WriteAsync(_packageBuffer, 0, packageBufferOffset, cancellationToken);
            await InnerStream.FlushAsync(cancellationToken);

            if (FlushCallback != null)
                await FlushCallback.Invoke();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).Wait();
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            _length += count;

            while (count > 0)
            {
                var spaceLeft = _packageBufferSize - _packageBufferOffset;

                //if we have enough space in the package buffer to fit the write buffer
                if (spaceLeft > count)
                {
                    CreateBufferIfNull();
                    Buffer.BlockCopy(buffer, offset, _packageBuffer, _packageBufferOffset, count);
                    _packageBufferOffset += count;
                    return;
                }

                if (_packageBufferOffset == 0)
                {
                    //if the stream buffer is empty and the write buffer exceeds the package buffer, we directly send from the write buffer
                    await InnerStream.WriteAsync(buffer, offset, _packageBufferSize, cancellationToken);
                    offset += _packageBufferSize;
                    count -= _packageBufferSize;
                }
                else
                {
                    //to keep the byte order, we must copy from the write buffer to the package buffer and send from there
                    Buffer.BlockCopy(buffer, offset, _packageBuffer, _packageBufferOffset, spaceLeft);
                    offset += spaceLeft;
                    count -= spaceLeft;
                    await InnerStream.WriteAsync(_packageBuffer, 0, _packageBufferSize, cancellationToken);

                    _packageBufferOffset = 0;
                }
            }
        }

        private void CreateBufferIfNull()
        {
            if (_packageBuffer == null)
                _packageBuffer = _bufferPool.Rent(_packageBufferSize);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;

                InnerStream.Dispose();

                if (_packageBuffer != null)
                    _bufferPool.Return(_packageBuffer);
            }
        }

        public override bool CanSeek { get; } = false;
        public override long Length => _length;
        public override long Position
        {
            get => _length;
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}