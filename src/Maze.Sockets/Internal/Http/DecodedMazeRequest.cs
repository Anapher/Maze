using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Maze.Modules.Api;
using Maze.Modules.Api.Request;

namespace Maze.Sockets.Internal.Http
{
    internal class DecodedMazeRequest : MazeRequest
    {
        public DecodedMazeRequest()
        {
            Headers = new HeaderDictionary();
        }

        public override MazeContext Context { get; set; }
        public override string Method { get; set; }
        public override PathString Path { get; set; }
        public override QueryString QueryString { get; set; }
        public override IQueryCollection Query { get; set; }
        public override IHeaderDictionary Headers { get; }

        public override long? ContentLength
        {
            get => Headers.ContentLength;
            set => Headers.ContentLength = value;
        }

        public override string ContentType
        {
            get => Headers[HeaderNames.ContentType];
            set => Headers[HeaderNames.ContentType] = value;
        }

        public override Stream Body { get; set; }
    }
}