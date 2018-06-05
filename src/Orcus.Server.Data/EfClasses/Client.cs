using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orcus.Server.Data.EfClasses
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required]
        public string Username { get; set; }

        public string OperatingSystem { get; set; }
        public string MacAddress { get; set; }
        public string SystemLanguage { get; set; }

        [Required]
        public string HardwareId { get; set; }

        public DateTimeOffset CreatedOn { get; private set; }

        public ICollection<ClientSession> ClientSessions { get; set; }
    }
}