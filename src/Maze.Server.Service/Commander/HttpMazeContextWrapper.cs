using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Maze.Modules.Api;
using Maze.Modules.Api.Request;
using Maze.Modules.Api.Response;

namespace Maze.Server.Service.Commander
{
    public class HttpMazeContextWrapper : MazeContext
    {
        private readonly HttpContext _httpContext;

        public HttpMazeContextWrapper(HttpContext httpContext)
        {
            _httpContext = httpContext;
            Request = new HttpMazeRequestWrapper(httpContext.Request);
            Response = new HttpMazeResponseWrapper(httpContext.Response);
        }

        public override object Caller { get; set; }
        public override MazeRequest Request { get; set; }
        public override MazeResponse Response { get; set; }
        public override ConnectionInfo Connection { get; set; }

        public override IServiceProvider RequestServices
        {
            get => _httpContext.RequestServices;
            set => _httpContext.RequestServices = value;
        }

        public override CancellationToken RequestAborted
        {
            get => _httpContext.RequestAborted;
            set => _httpContext.RequestAborted = value;
        }

        public override void Abort()
        {
            _httpContext.Abort();
        }
    }
}
