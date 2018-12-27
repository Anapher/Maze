namespace Orcus.Sockets
{
    public class DataReceivedEventArgs
    {
        public DataReceivedEventArgs(BufferSegment buffer, OrcusSocket.MessageOpcode opcode)
        {
            Buffer = buffer;
            Opcode = opcode;
        }

        public BufferSegment Buffer { get; }
        public OrcusSocket.MessageOpcode Opcode { get; }
    }
}