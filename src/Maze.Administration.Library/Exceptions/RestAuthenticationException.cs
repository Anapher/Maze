using Maze.Server.Connection;

namespace Maze.Administration.Library.Exceptions
{
    public class RestAuthenticationException : RestException
    {
        public RestAuthenticationException(RestError error) : base(error)
        {
        }
    }
}