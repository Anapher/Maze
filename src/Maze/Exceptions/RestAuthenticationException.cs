using Maze.Server.Connection;

namespace Maze.Exceptions
{
    public class RestAuthenticationException : RestException
    {
        public RestAuthenticationException(RestError error) : base(error)
        {
        }
    }
}