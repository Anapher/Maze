using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RequestTransmitter.Client.Utilities
{
    //Taken from https://stackoverflow.com/a/3879231
    public class ConcatenatedStream : Stream
    {
        private readonly Queue<Stream> _streams;

        public ConcatenatedStream(IEnumerable<Stream> streams)
        {
            this._streams = new Queue<Stream>(streams);
        }

        public override bool CanRead => true;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_streams.Count == 0)
                return 0;

            int bytesRead = _streams.Peek().Read(buffer, offset, count);
            if (bytesRead == 0)
            {
                _streams.Dequeue().Dispose();
                bytesRead += Read(buffer, offset + bytesRead, count - bytesRead);
            }
            return bytesRead;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_streams.Count == 0)
                return 0;

            int bytesRead = await _streams.Peek().ReadAsync(buffer, offset, count, cancellationToken);
            if (bytesRead == 0)
            {
                _streams.Dequeue().Dispose();
                bytesRead += await ReadAsync(buffer, offset + bytesRead, count - bytesRead, cancellationToken);
            }
            return bytesRead;
        }

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
