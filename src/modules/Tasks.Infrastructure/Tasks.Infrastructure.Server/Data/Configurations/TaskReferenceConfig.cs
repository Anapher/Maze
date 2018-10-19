using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tasks.Infrastructure.Core.Data;

namespace Tasks.Infrastructure.Server.Data.Configurations
{
   internal class TaskReferenceConfig : IEntityTypeConfiguration<TaskReference>
    {
        public void Configure(EntityTypeBuilder<TaskReference> builder)
        {
            builder.HasIndex(x => x.TaskId).IsUnique();
            builder.HasIndex(x => x.Filename).IsUnique();
        }
    }
}