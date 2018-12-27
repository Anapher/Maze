using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Data.EfCode.Configurations
{
    internal class ClientConfig : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasIndex(x => x.HardwareId).IsUnique();
            builder.Property(x => x.HardwareId).IsSha256Hash();
            builder.Property(x => x.CreatedOn).IsCurrentTime();
        }
    }
}