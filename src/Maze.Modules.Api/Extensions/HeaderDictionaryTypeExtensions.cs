using Microsoft.AspNetCore.Http.Headers;
using Maze.Modules.Api.Request;
using Maze.Modules.Api.Response;

namespace Maze.Modules.Api.Extensions
{
    public static class HeaderDictionaryTypeExtensions
    {
        public static RequestHeaders GetTypedHeaders(this MazeRequest request)
        {
            return new RequestHeaders(request.Headers);
        }

        public static ResponseHeaders GetTypedHeaders(this MazeResponse request)
        {
            return new ResponseHeaders(request.Headers);
        }
    }
}