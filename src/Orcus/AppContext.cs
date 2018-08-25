using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Autofac;
using Microsoft.Extensions.Options;
using NuGet.Frameworks;
using NuGet.Versioning;
using Orcus.Client.Library.Interfaces;
using Orcus.Core;
using Orcus.Core.Connection;
using Orcus.Core.Modules;
using Orcus.ModuleManagement;
using Orcus.Options;

namespace Orcus
{
    public class AppContext : ApplicationContext, IApplicationInfo
    {
        public AppContext(ContainerBuilder builder)
        {
            builder.RegisterInstance(this).AsImplementedInterfaces();
            RootContainer = builder.Build();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

            Container = LoadModules();
            StartConnecting();

            Container.Execute<IStartupAction>();
        }

        /// <summary>
        ///     The root container that contains all services of Orcus
        /// </summary>
        private IContainer RootContainer { get; }

        /// <summary>
        ///     The core container, which inherits from <see cref="RootContainer" /> but also provides services from local modules
        /// </summary>
        public ILifetimeScope Container { get; }

        public NuGetFramework Framework { get; } = FrameworkConstants.CommonFrameworks.OrcusClient10;
        public NuGetVersion Version { get; } = NuGetVersion.Parse("1.0");

        private ILifetimeScope LoadModules()
        {
            var loader = RootContainer.Resolve<IPackageLockLoader>();

            var modulesConfig = RootContainer.Resolve<IOptions<ModulesOptions>>().Value;

            var loadContext = loader.Load(modulesConfig.PackagesLock).Result;
            if (loadContext.PackagesLoaded)
            {
                return RootContainer.BeginLifetimeScope(builder => loadContext.Configure(builder));
            }

            return RootContainer;
        }

        private void StartConnecting()
        {
            Container.Resolve<ICoreConnector>().StartConnecting(Container);
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == args.Name);
        }
    }
}