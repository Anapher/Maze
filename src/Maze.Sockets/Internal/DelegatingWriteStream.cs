using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Maze.Sockets.Internal.Infrastructure;

namespace Maze.Sockets.Internal
{
    /// <summary>
    ///     A write-only stream that delegates write accesses
    /// </summary>
    public class DelegatingWriteStream : WriteOnlyStream
    {
        public OnSendPackageDelegate SendPackageDelegate { get; }

        public DelegatingWriteStream(OnSendPackageDelegate sendPackageDelegate)
        {
            SendPackageDelegate = sendPackageDelegate;
        }

        public override void Flush()
        {
            FlushAsync(CancellationToken.None).Wait();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            SendPackageDelegate(new ArraySegment<byte>());
            return Task.CompletedTask;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).Wait();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return SendPackageDelegate(new ArraySegment<byte>(buffer, offset, count));
        }

        public override bool CanSeek { get; } = false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}