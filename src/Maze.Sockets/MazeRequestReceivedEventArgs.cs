using System;
using System.Threading;
using Maze.Modules.Api.Request;
using Maze.Modules.Api.Response;

namespace Maze.Sockets
{
    public class MazeRequestReceivedEventArgs : EventArgs
    {
        public MazeRequestReceivedEventArgs(MazeRequest request, MazeResponse response,
            CancellationToken cancellationToken)
        {
            Request = request;
            Response = response;
            CancellationToken = cancellationToken;
        }

        public MazeRequest Request { get; }
        public MazeResponse Response { get; }
        public CancellationToken CancellationToken { get; }
    }
}