using System.Net;
using Maze.Server.Connection.Authentication.Client;

namespace Maze.Server.BusinessLogic.Authentication
{
    public class ClientAuthenticationContext
    {
        public ClientAuthenticationDto Dto { get; set; }
        public IPAddress IpAddress { get; set; }
    }
}