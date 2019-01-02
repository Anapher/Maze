using System;
using System.IO;
using System.Windows.Forms;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Maze
{
    internal static class Program
    {
        public const string ConfigDirectory = "%appdata%/Maze";

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Debug(LogEventLevel.Debug).CreateLogger();

            var configBuilder = new ConfigurationBuilder();
#if DEBUG
            configBuilder.AddJsonFile("mazesettings.json");
#endif
            var configurationDirectory = new DirectoryInfo(Environment.ExpandEnvironmentVariables(ConfigDirectory));
            if (!configurationDirectory.Exists)
            {
                Log.Logger.Fatal("The directory {configDirectory} was not found so no configuration could be loaded", ConfigDirectory);
                Environment.Exit(-1);
                return;
            }

            foreach (var fileInfo in configurationDirectory.GetFiles("mazesettings*.json"))
                configBuilder.AddJsonFile(fileInfo.FullName, optional: true, reloadOnChange: true);

            var config = configBuilder.Build();
            var startup = new Startup(config);

            var builder = new ContainerBuilder();
            startup.ConfigureServices(builder);

            Application.Run(new AppContext(builder));
        }
    }
}