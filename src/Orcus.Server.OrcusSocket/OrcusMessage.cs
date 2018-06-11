using System;
using System.Buffers;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusMessage : IDisposable
    {
        private bool _disposed;

        public ArraySegment<byte> Buffer { get; set; }
        public uint ChannelId { get; set; }

        public void Dispose()
        {
            if (_disposed)
            {
                ArrayPool<byte>.Shared.Return(Buffer.Array);
                _disposed = true;
            }
        }
    }
}