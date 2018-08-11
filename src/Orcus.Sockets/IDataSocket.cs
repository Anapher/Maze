using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Sockets
{
    public interface IDataSocket : IDisposable
    {
        Task SendFrameAsync(OrcusSocket.MessageOpcode opcode, ArraySegment<byte> payloadBuffer,
            CancellationToken cancellationToken);

        event EventHandler<DataReceivedEventArgs> DataReceivedEventArgs;
    }
}
