using System;

namespace Maze.Server.Connection.Clients
{
    public class ClientSessionDto
    {
        public int ClientSessionId { get; set; }

        public bool IsAdministrator { get; set; }
        public string ClientVersion { get; set; }
        public string ClientPath { get; set; }
        public string IpAddress { get; set; }

        public DateTimeOffset CreatedOn { get; private set; }
    }
}