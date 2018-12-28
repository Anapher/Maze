using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Maze.Server.Data.EfClasses;

namespace Maze.Server.Data.EfCode.Configurations
{
    internal class ClientSessionConfig : IEntityTypeConfiguration<ClientSession>
    {
        public void Configure(EntityTypeBuilder<ClientSession> builder)
        {
            builder.Property(x => x.CreatedOn).IsCurrentTime();
        }
    }
}