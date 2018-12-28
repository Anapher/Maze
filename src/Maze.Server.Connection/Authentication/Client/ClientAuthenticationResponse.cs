using Maze.Server.Connection.Modules;

namespace Maze.Server.Connection.Authentication.Client
{
    public class ClientAuthenticationResponse
    {
        public string Jwt { get; set; }
        public PackagesLock Modules { get; set; }
    }
}