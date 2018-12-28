using Maze.Server.Connection;

namespace Maze.Administration.Library.Exceptions
{
    public class RestNotFoundException : RestException
    {
        public RestNotFoundException(RestError error) : base(error)
        {
        }
    }
}