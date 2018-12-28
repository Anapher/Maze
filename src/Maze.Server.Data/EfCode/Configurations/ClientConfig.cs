using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.Data.EfCode.Configurations
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