using System;
using System.ComponentModel.DataAnnotations;

namespace Maze.Server.Data.EfClasses
{
    public class ClientConfiguration
    {
        public int ClientConfigurationId { get; set; }
        public int? ClientGroupId { get; set; }

        [Required]
        public string Content { get; set; }

        public int ContentHash { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public ClientGroup ClientGroup { get; set; }
    }
}