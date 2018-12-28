using Maze.Server.Connection;

namespace Maze.Server.Library.Exceptions
{
    public class RestAuthenticationException : RestException
    {
        public RestAuthenticationException(RestError error) : base(error)
        {
        }
    }
}