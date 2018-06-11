using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusSocketResponseStream : Stream
    {
        public delegate Task OnSendDataDelegate(ArraySegment<byte> data);

        private long _length;
        private readonly OnSendDataDelegate _onSendData;

        public OrcusSocketResponseStream(OnSendDataDelegate onSendData)
        {
            _onSendData = onSendData;
        }

        public override void Flush()
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            _length += count;
            return _onSendData.Invoke(new ArraySegment<byte>(buffer, offset, count));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead { get; } = false;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;
        public override long Length => _length;

        public override long Position
        {
            get => _length;
            set => throw new NotSupportedException();
        }
    }
}