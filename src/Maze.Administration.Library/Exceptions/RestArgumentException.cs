using Maze.Server.Connection;

namespace Maze.Administration.Library.Exceptions
{
    public class RestArgumentException : RestException
    {
        public RestArgumentException(RestError error) : base(error)
        {
        }
    }
}