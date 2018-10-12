using Microsoft.EntityFrameworkCore;
using Orcus.Server.Data.EfClasses;
using Orcus.Server.Data.Extensions;

namespace Orcus.Server.Data.EfCode
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyAllConfigurations();
        }
    }
}