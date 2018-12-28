using System.IO;
using Microsoft.AspNetCore.Http;
using Maze.Modules.Api;
using Maze.Modules.Api.Request;

namespace Maze.Server.Service.Commander
{
    public class HttpMazeRequestWrapper : MazeRequest
    {
        private readonly HttpRequest _httpRequest;

        public HttpMazeRequestWrapper(HttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }

        public override MazeContext Context { get; set; }

        public override string Method
        {
            get => _httpRequest.Method;
            set => _httpRequest.Method = value;
        }

        public override PathString Path
        {
            get => _httpRequest.Path;
            set => _httpRequest.Path = value;
        }

        public override QueryString QueryString
        {
            get => _httpRequest.QueryString;
            set => _httpRequest.QueryString = value;
        }

        public override IQueryCollection Query
        {
            get => _httpRequest.Query;
            set => _httpRequest.Query = value;
        }

        public override IHeaderDictionary Headers => _httpRequest.Headers;

        public override long? ContentLength
        {
            get => _httpRequest.ContentLength;
            set => _httpRequest.ContentLength = value;
        }

        public override string ContentType
        {
            get => _httpRequest.ContentType;
            set => _httpRequest.ContentType = value;
        }

        public override Stream Body
        {
            get => _httpRequest.Body;
            set => _httpRequest.Body = value;
        }
    }
}