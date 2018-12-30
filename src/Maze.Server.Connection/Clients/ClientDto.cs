using System;

namespace Maze.Server.Connection.Clients
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string Username { get; set; }
        public string OperatingSystem { get; set; }
        public string MacAddress { get; set; }
        public string SystemLanguage { get; set; }
        public string HardwareId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public bool IsSocketConnected { get; set; }
        public ClientSessionDto LatestSession { get; set; }
    }
}