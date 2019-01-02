using System;
using System.Buffers;
using System.IO.Abstractions;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Maze.Client.Library.Extensions;
using Maze.Client.Library.Services;
using Maze.Core.Configuration;
using Maze.Core.Connection;
using Maze.Core.Modules;
using Maze.Core.Services;
using Maze.Core.Startup;
using Maze.Extensions;
using Maze.ModuleManagement;
using Maze.Options;
using Maze.Server.Connection.Utilities;

namespace Maze
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
            builder.RegisterType<MazeRestClientFactory>().As<IMazeRestClientFactory>();
            builder.RegisterType<ServerConnector>().As<IServerConnector>();
            builder.RegisterType<ModuleDownloader>().As<IModuleDownloader>();
            builder.RegisterType<CoreConnector>().As<ICoreConnector>().As<IManagementCoreConnector>().SingleInstance();
            builder.RegisterType<ClientInfoProvider>().As<IClientInfoProvider>().SingleInstance();
            builder.RegisterType<XmlSerializerCache>().As<IXmlSerializerCache>().SingleInstance();
            builder.RegisterType<ConfigurationManager>().As<IConfigurationManager>().SingleInstance();
            builder.RegisterType<StartupActionInvoker>().As<IStartupActionInvoker>();
            builder.RegisterType<PackageLockUpdater>().As<IPackageLockUpdater>();
            builder.RegisterType<MazeRestClientWrapper>().AsSelf().AsImplementedInterfaces().SingleInstance();

            builder.RegisterInstance(new SerilogLoggerFactory()).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            builder.RegisterInstance(ArrayPool<char>.Create());
            builder.RegisterInstance(ArrayPool<byte>.Create());

            
            builder.Populate(Enumerable.Empty<ServiceDescriptor>());

            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            
            builder.RegisterModule<ModuleManagementModule>();
        }
    }
}