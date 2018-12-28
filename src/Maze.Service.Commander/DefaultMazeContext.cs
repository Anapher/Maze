using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Maze.Modules.Api;
using Maze.Modules.Api.Request;
using Maze.Modules.Api.Response;

namespace Maze.Service.Commander
{
    public class DefaultMazeContext : MazeContext
    {
        private readonly CancellationTokenSource _requestCancellationTokenSource;

        public DefaultMazeContext(MazeRequest request, MazeResponse response, IServiceProvider serviceProvider)
        {
            Request = request;
            Response = response;

            Request.Context = this;

            RequestServices = serviceProvider;

            _requestCancellationTokenSource = new CancellationTokenSource();
            RequestAborted = _requestCancellationTokenSource.Token;
        }

        public override object Caller { get; set; }

        public override MazeResponse Response { get; set; }
        public override MazeRequest Request { get; set; }
        public override ConnectionInfo Connection { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }

        public override void Abort()
        {
            _requestCancellationTokenSource.Cancel();
        }
    }
}