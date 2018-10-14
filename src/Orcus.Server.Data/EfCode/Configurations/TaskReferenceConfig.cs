using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orcus.Server.Data.EfClasses.Tasks;

namespace Orcus.Server.Data.EfCode.Configurations
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