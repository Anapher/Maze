using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Server.OrcusSockets.Internal.Infrastructure
{
    public abstract class ReadOnlyStream : Stream
    {
        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override int WriteTimeout
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }
}