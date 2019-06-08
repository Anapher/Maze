using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Maze.Server.Connection.Utilities;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;
using Microsoft.Extensions.Logging;

namespace Maze.Server.AppStart
{
    public static class AppContextDbSeed
    {
        public static void EnsureDataSeeded(this AppDbContext context, ILogger logger)
        {
            context.SeedClientConfiguration(logger);
            context.SeedAccount(logger);
            context.SaveChanges();
        }

        public static void SeedAccount(this AppDbContext context, ILogger logger)
        {
            if (!context.Set<Account>().Any())
            {
                logger.LogWarning("No accounts found. Creating new account with credentials admin:admin.\r\nPlease change the password of the default account immediately after logging in!");

                context.Add(new Account { IsEnabled = true, Username = "admin", Password = BCrypt.Net.BCrypt.HashPassword("admin") });
            }
        }

        private static void SeedClientConfiguration(this AppDbContext context, ILogger logger)
        {
            if (context.Set<ClientConfiguration>().Any(x => x.ClientGroupId == null))
                return;

            logger.LogInformation("Client Configuration not seeded. Applying...");

            string defaultConfig;
            using (var contentStream = Assembly.GetEntryAssembly().GetManifestResourceStream("Maze.Server.mazesettings.json"))
            using (var streamReader = new StreamReader(contentStream))
            {
                defaultConfig = streamReader.ReadToEnd();
            }

            var clientConfiguration = new ClientConfiguration
            {
                Content = defaultConfig, ContentHash = (int) MurmurHash2.Hash(defaultConfig), UpdatedOn = DateTimeOffset.UtcNow
            };

            context.Add(clientConfiguration);

            logger.LogInformation("Default Client Configuration added");
        }
    }
}