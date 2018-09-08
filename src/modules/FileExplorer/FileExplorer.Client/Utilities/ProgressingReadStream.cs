using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Client.Utilities
{
    public class ProgressingReadStream : Stream
    {
        private readonly Action<double> _progressChanged;
        private int _lastProgressChanged;

        public ProgressingReadStream(Stream baseStream, Action<double> progressChanged)
        {
            _progressChanged = progressChanged;
            BaseStream = baseStream;
        }

        public Stream BaseStream { get; }

        public override bool CanRead => true;
        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => false;
        public override long Length => BaseStream.Length;
        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public override int WriteTimeout
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var progress = Position / (double) Length;
            var progressRounded = (int) progress * 100;
            if (_lastProgressChanged != progressRounded)
            {
                _lastProgressChanged = progressRounded;
                _progressChanged(progress);
            }
            return BaseStream.Read(buffer, offset, count);
        }
    }
}