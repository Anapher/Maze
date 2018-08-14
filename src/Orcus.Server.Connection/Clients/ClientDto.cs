using System;

namespace Orcus.Server.Connection.Clients
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