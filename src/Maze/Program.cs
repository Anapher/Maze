using System;
using System.Windows.Forms;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Orcus
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Debug(LogEventLevel.Debug).CreateLogger();

            var config = new ConfigurationBuilder().AddJsonFile("orcussettings.json").Build();
            var startup = new Startup(config);

            var builder = new ContainerBuilder();
            startup.ConfigureServices(builder);

            Application.Run(new AppContext(builder));
        }
    }
}