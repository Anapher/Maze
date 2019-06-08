using System;
using Maze.Server.AppStart;
using Maze.Server.Data.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Maze.Server
{
    public static class DatabaseSeedInitializer
    {
        public static IWebHost Seed(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    var database = serviceProvider.GetRequiredService<AppDbContext>();
                    database.Database.Migrate();

                    database.EnsureDataSeeded(logger);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the Db.");
                    throw;
                }
            }

            return host;
        }
    }
}