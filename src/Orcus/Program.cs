using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Orcus.Core.Modules;
using Orcus.Options;

namespace Orcus
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static async Task Main()
        {
            var config = new ConfigurationBuilder().AddJsonFile("orcussettings.json").Build();
            var startup = new Startup(config);

            var builder = new ContainerBuilder();
            startup.ConfigureServices(builder);

            ConfigureModules(builder, config);

            var container = builder.Build();
        }

        private static void ConfigureModules(ContainerBuilder builder, IConfiguration configuration)
        {
            var tempContainer = builder.Build();

            var option = tempContainer.Resolve<IOptions<ModulesOptions>>();

            var catalog = tempContainer.Resolve<IModulesCatalog>();
            var localPackages = catalog.Load(option.Value.GetPrimaryModules(), option.Value.GetPackagesLock());

            //https://github.com/autofac/Autofac/blob/41044d7d1a4fa277c628021537d5a12016137c3b/src/Autofac/ModuleRegistrationExtensions.cs#L156
            var moduleFinder = new ContainerBuilder();
            moduleFinder.RegisterInstance(configuration);

            moduleFinder.RegisterAssemblyTypes(localPackages.Select(x => x.Assembly).ToArray())
                .Where(t => typeof(IModule).IsAssignableFrom(t))
                .As<IModule>();

            IModuleRegistrar registrar = null;
            using (var moduleContainer = moduleFinder.Build())
            {
                foreach (var module in moduleContainer.Resolve<IEnumerable<IModule>>())
                {
                    if (registrar == null)
                        registrar = builder.RegisterModule(module);
                    else
                        registrar.RegisterModule(module);
                }
            }
        }
    }
}