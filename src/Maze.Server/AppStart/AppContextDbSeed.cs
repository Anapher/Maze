using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Maze.Server.Connection.Utilities;
using Maze.Server.Data.EfClasses;
using Maze.Server.Data.EfCode;

namespace Maze.Server.AppStart
{
    public static class AppContextDbSeed
    {
        public static void EnsureDataSeeded(this AppDbContext context)
        {
            context.SeedClientConfiguration();
            context.SaveChanges();
        }

        private static void SeedClientConfiguration(this AppDbContext context)
        {
            if (context.Set<ClientConfiguration>().Any(x => x.ClientGroupId == null))
                return;

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
        }
    }
}