using Maze.Server.Connection;

namespace Maze.Server.Library.Exceptions
{
    public class RestArgumentException : RestException
    {
        public RestArgumentException(RestError error) : base(error)
        {
        }
    }
}