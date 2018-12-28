using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.Data.EfCode.Configurations
{
    internal class ClientConfigurationConfig : IEntityTypeConfiguration<ClientConfiguration>
    {
        public void Configure(EntityTypeBuilder<ClientConfiguration> builder)
        {
            builder.HasIndex(x => x.ClientGroupId).IsUnique();
        }
    }
}