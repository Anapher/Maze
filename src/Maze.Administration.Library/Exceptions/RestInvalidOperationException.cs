using Maze.Server.Connection;

namespace Maze.Administration.Library.Exceptions
{
    public class RestInvalidOperationException : RestException
    {
        public RestInvalidOperationException(RestError error) : base(error)
        {
        }
    }
}