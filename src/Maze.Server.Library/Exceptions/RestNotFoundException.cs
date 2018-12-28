using Maze.Server.Connection;

namespace Maze.Server.Library.Exceptions
{
    public class RestNotFoundException : RestException
    {
        public RestNotFoundException(RestError error) : base(error)
        {
        }
    }
}