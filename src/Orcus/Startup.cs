using Autofac;
using Microsoft.Extensions.Configuration;
using Orcus.Client.Library.Extensions;
using Orcus.Core.Modules;
using Orcus.ModuleManagement;
using Orcus.Options;

namespace Orcus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.AddOptions();

            builder.Configure<ModulesOptions>(Configuration.GetSection("Modules"));

            builder.RegisterType<ClientModulesDirectory>().As<IModulesDirectory>().SingleInstance();
            builder.RegisterType<PackageLoader>().As<IPackageLoader>().SingleInstance();
            builder.RegisterType<ModulesCatalog>().As<IModulesCatalog>().SingleInstance();



        }
    }
}