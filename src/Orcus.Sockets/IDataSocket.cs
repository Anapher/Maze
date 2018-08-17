using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Sockets
{
    public interface IDataSocket : IDisposable
    {
        ArrayPool<byte> BufferPool { get; }

        event EventHandler<DataReceivedEventArgs> DataReceivedEventArgs;

        Task SendFrameAsync(OrcusSocket.MessageOpcode opcode, ArraySegment<byte> payloadBuffer,
            CancellationToken cancellationToken);
    }
}