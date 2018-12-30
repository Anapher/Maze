using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Maze.Server
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                BuildWebHost(args).Seed().Run();
                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
        }
    }
}