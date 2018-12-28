using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.Data.EfCode.Configurations
{
    internal class ClientGroupConfig : IEntityTypeConfiguration<ClientGroup>
    {
        public void Configure(EntityTypeBuilder<ClientGroup> builder)
        {
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}