using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orcus.Server.Data.EfCode;
using Tasks.Infrastructure.Core.Data;

namespace Tasks.Infrastructure.Server.Data.Configurations
{
    internal class TaskSessionConfig : IEntityTypeConfiguration<TaskSession>
    {
        public void Configure(EntityTypeBuilder<TaskSession> builder)
        {
            builder.Property(x => x.CreatedOn).IsCurrentTime();
        }
    }
}
