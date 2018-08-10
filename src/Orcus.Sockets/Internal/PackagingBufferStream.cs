using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Sockets.Internal.Infrastructure;

namespace Orcus.Sockets.Internal
{
    public delegate Task OnSendPackageDelegate(ArraySegment<byte> data);

    public class PackagingBufferStream : WriteOnlyStream
    {
        private readonly OnSendPackageDelegate _sendPackageDelegate;
        private readonly int _packageBufferSize;
        private long _length;
        private byte[] _packageBuffer;
        private int _packageBufferOffset;

        public PackagingBufferStream(OnSendPackageDelegate sendPackageDelegate, int packageBufferSize)
        {
            _sendPackageDelegate = sendPackageDelegate;
            _packageBufferSize = packageBufferSize;
        }

        public override void Flush()
        {
            FlushAsync().Wait();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (_packageBufferOffset == 0)
                return _sendPackageDelegate(new ArraySegment<byte>());

            var packageBufferOffset = _packageBufferOffset;
            _packageBufferOffset = 0;
            return _sendPackageDelegate(new ArraySegment<byte>(_packageBuffer, 0, packageBufferOffset));
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
                if (spaceLeft >= count)
                {
                    CreateBufferIfNull();
                    Buffer.BlockCopy(buffer, offset, _packageBuffer, _packageBufferOffset, count);
                    _packageBufferOffset += count;
                    return;
                }

                if (_packageBufferOffset == 0)
                {
                    //if the stream buffer is empty and the write buffer exceeds the package buffer, we directly send from the write buffer
                    await _sendPackageDelegate(new ArraySegment<byte>(buffer, offset, _packageBufferSize));
                    offset += _packageBufferSize;
                    count -= _packageBufferSize;
                }
                else
                {
                    //to keep the byte order, we must copy from the write buffer to the package buffer and send from there
                    Buffer.BlockCopy(buffer, offset, _packageBuffer, _packageBufferOffset, spaceLeft);
                    offset += spaceLeft;
                    count -= spaceLeft;
                    await _sendPackageDelegate(new ArraySegment<byte>(_packageBuffer, 0, _packageBufferSize));

                    _packageBufferOffset = 0;
                }
            }
        }

        private void CreateBufferIfNull()
        {
            if (_packageBuffer == null)
                _packageBuffer = ArrayPool<byte>.Shared.Rent(_packageBufferSize);
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