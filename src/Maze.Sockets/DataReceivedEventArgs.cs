namespace Maze.Sockets
{
    public class DataReceivedEventArgs
    {
        public DataReceivedEventArgs(BufferSegment buffer, MazeSocket.MessageOpcode opcode)
        {
            Buffer = buffer;
            Opcode = opcode;
        }

        public BufferSegment Buffer { get; }
        public MazeSocket.MessageOpcode Opcode { get; }
    }
}