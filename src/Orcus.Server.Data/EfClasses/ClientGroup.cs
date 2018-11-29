using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orcus.Server.Data.EfClasses
{
    public class ClientGroup
    {
        public int ClientGroupId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<ClientGroupMembership> ClientGroupMemberships { get; set; }
    }
}
