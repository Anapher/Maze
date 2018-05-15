using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orcus.Server.Data.EfClasses;

namespace Orcus.Server.Data.EfCode.Configurations
{
    internal class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.Property(x => x.CreatedOn).IsCurrentTime();
            builder.Property(x => x.TokenValidityPeriod).IsCurrentTime();
            builder.HasIndex(x => x.Username).IsUnique();
            builder.Property(x => x.Password).IsFixedLengthString(60);
        }
    }
}