using System;
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

        [Required]
        public string HardwareId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastLogin { get; set; }
    }
}