using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Data.EfCode.Configurations
{
    internal class ClientGroupMembershipConfig : IEntityTypeConfiguration<ClientGroupMembership>
    {
        public void Configure(EntityTypeBuilder<ClientGroupMembership> builder)
        {
            builder.HasKey(x => new {x.ClientId, x.ClientGroupId});
        }
    }
}