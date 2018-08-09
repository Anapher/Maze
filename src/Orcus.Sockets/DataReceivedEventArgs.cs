using System;
using System.Buffers;

namespace Orcus.Sockets
{
    public class DataReceivedEventArgs : IDisposable
    {
        public DataReceivedEventArgs(ArraySegment<byte> buffer, OrcusSocket.MessageOpcode opcode)
        {
            Buffer = buffer;
            Opcode = opcode;
        }

        public ArraySegment<byte> Buffer { get; }
        public OrcusSocket.MessageOpcode Opcode { get; }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffer.Array);
        }
    }
}