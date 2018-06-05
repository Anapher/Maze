using System;

namespace Orcus.Server.Data.EfClasses
{
    public class ClientSession
    {
        public int ClientSessionId { get; set; }
        public int ClientId { get; set; }

        public bool IsAdministrator { get; set; }
        public string ClientVersion { get; set; }
        public string ClientPath { get; set; }
        public string IpAddress { get; set; }

        public DateTimeOffset CreatedOn { get; private set; }

        public Client Client { get; set; }
    }
}