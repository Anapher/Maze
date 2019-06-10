using System;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Maze.Server.Library.Interfaces;
using Tasks.Infrastructure.Server.Core;
using Tasks.Infrastructure.Server.Migrations;
using Tasks.Infrastructure.Server.Options;
using System.IO;

namespace Tasks.Infrastructure.Server.Hooks
{
    public class OnServerStartupEvent : IConfigureServerPipelineAction
    {
        private readonly TasksOptions _options;
        private readonly IMazeTaskManager _mazeTaskManager;

        public OnServerStartupEvent(IMazeTaskManager mazeTaskManager, IOptions<TasksOptions> options)
        {
            _mazeTaskManager = mazeTaskManager;
            _options = options.Value;
        }

        public Task Execute(PipelineInfo context)
        {
            Directory.CreateDirectory(_options.Directory);

            var serviceProvider = CreateServices();

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }

            return _mazeTaskManager.Initialize();
        }

        /// <summary>
        ///     Configure the dependency injection services
        /// </summary>
        private IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore().ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddSQLite()
                    // Set the connection string
                    .WithGlobalConnectionString(_options.ConnectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(InitialCreate).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging()
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        ///     Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }
    }
}