using System.Buffers;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orcus.Client.Library.Extensions;
using Orcus.Client.Library.Services;
using Orcus.Core.Connection;
using Orcus.Core.Modules;
using Orcus.Core.Services;
using Orcus.Extensions;
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
            builder.Configure<ConnectionOptions>(Configuration.GetSection("Connection"));

            builder.RegisterType<ClientModulesDirectory>().As<IModulesDirectory>().SingleInstance();
            builder.RegisterType<PackageLoader>().As<IPackageLoader>().SingleInstance();
            builder.RegisterType<ModulesCatalog>().As<IModulesCatalog>().SingleInstance();
            builder.RegisterInstance(new ConfigurationRootProvider(Configuration)).As<IConfigurationRootProvider>();
            builder.RegisterType<PackageLockLoader>().As<IPackageLockLoader>();
            builder.RegisterType<PackagesRegistrar>().As<IPackagesRegistrar>();
            builder.RegisterType<HttpClientService>().As<IHttpClientService>().SingleInstance();
            builder.RegisterType<OrcusRestClientFactory>().As<IOrcusRestClientFactory>();
            builder.RegisterType<ServerConnector>().As<IServerConnector>();
            builder.RegisterType<ModuleDownloader>().As<IModuleDownloader>();
            builder.RegisterType<CoreConnector>().As<ICoreConnector>().As<IManagementCoreConnector>().SingleInstance();
            builder.RegisterType<ClientInfoProvider>().As<IClientInfoProvider>().SingleInstance();

            builder.RegisterInstance(new SerilogLoggerFactory()).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            builder.RegisterInstance(ArrayPool<char>.Create());
            builder.RegisterInstance(ArrayPool<byte>.Create());
            
            builder.RegisterModule<ModuleManagementModule>();
        }
    }
}