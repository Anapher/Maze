using Autofac;
using Microsoft.Extensions.Configuration;
using Orcus.Client.Library.Extensions;
using Orcus.Client.Library.Services;
using Orcus.Clients;
using Orcus.Core.Connection;
using Orcus.Core.Modules;
using Orcus.Core.Services;
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
            builder.RegisterInstance(new ConfigurationRootProvider(Configuration)).As<IConfigurationRootProvider>();
            builder.RegisterType<LocalModuleLoader>().As<ILocalModuleLoader>();
            builder.RegisterType<PackagesRegistrar>().As<IPackagesRegistrar>();
            builder.RegisterType<HttpClientService>().As<IHttpClientService>().SingleInstance();
            builder.RegisterType<OrcusRestClientFactory>().As<IOrcusRestClientFactory>();
            builder.RegisterType<ServerConnector>().As<IServerConnector>();
            builder.RegisterType<ModuleDownloader>().As<IModuleDownloader>();
        }
    }
}