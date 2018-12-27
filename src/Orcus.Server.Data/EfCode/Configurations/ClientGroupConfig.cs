using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Data.EfCode.Configurations
{
    internal class ClientGroupConfig : IEntityTypeConfiguration<ClientGroup>
    {
        public void Configure(EntityTypeBuilder<ClientGroup> builder)
        {
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}