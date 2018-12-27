using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Data.EfCode.Configurations
{
    internal class ClientSessionConfig : IEntityTypeConfiguration<ClientSession>
    {
        public void Configure(EntityTypeBuilder<ClientSession> builder)
        {
            builder.Property(x => x.CreatedOn).IsCurrentTime();
        }
    }
}