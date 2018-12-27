using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Data.EfCode.Configurations
{
    internal class ClientConfigurationConfig : IEntityTypeConfiguration<ClientConfiguration>
    {
        public void Configure(EntityTypeBuilder<ClientConfiguration> builder)
        {
            builder.HasIndex(x => x.ClientGroupId).IsUnique();
        }
    }
}