using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Maze.Modules.Api;
using Maze.Modules.Api.Request;
using Maze.Modules.Api.Response;
using Maze.Sockets;

namespace Maze.Core.Commanding
{
    public class WebSocketMazeContext : MazeContext
    {
        public WebSocketMazeContext(MazeRequestReceivedEventArgs args)
        {
            Request = args.Request;
            Response = args.Response;
            RequestAborted = args.CancellationToken;

            Request.Context = this;
        }

        public override object Caller { get; set; }
        public override MazeRequest Request { get; set; }
        public override MazeResponse Response { get; set; }
        public override ConnectionInfo Connection { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }

        public override void Abort()
        {
        }
    }
}