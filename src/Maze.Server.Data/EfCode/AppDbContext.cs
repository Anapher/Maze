using Microsoft.EntityFrameworkCore;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.Extensions;
using Maze.Server.Data.EfCode.Configurations;

namespace Maze.Server.Data.EfCode
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientGroup> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyAllConfigurations();
        }
    }
}