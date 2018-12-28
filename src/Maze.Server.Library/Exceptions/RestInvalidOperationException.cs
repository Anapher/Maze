using Maze.Server.Connection;

namespace Maze.Server.Library.Exceptions
{
    public class RestInvalidOperationException : RestException
    {
        public RestInvalidOperationException(RestError error) : base(error)
        {
        }
    }
}