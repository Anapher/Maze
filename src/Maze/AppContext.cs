using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using Maze.Client.Library.Interfaces;
using Maze.Client.Library.Services;
using Maze.Core.Connection;
using Maze.Core.Modules;
using Maze.Core.Startup;
using Maze.ModuleManagement;
using Maze.Options;
using Maze.Server.Connection.Modules;
using Maze.Visibility;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using Newtonsoft.Json;
using NuGet.Frameworks;
using NuGet.Versioning;

namespace Maze
{
    public class AppContext : ApplicationContext, IApplicationInfo, IStaSynchronizationContext, IMazeProcessController
    {
        private bool _shutdownTriggered;
        private readonly NotificationIcon _notificationIcon = new NotificationIcon();

        public AppContext(ContainerBuilder builder)
        {
            builder.RegisterInstance(this).AsImplementedInterfaces();
            RootContainer = builder.Build();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

            Container = LoadModules();
            StartConnecting();

            Container.Resolve<IStartupActionInvoker>().Load(CancellationToken.None);

            _notificationIcon.InitializeComponent();

            Application.Idle += ApplicationOnIdle;
            Application.ApplicationExit += ApplicationOnApplicationExit;
        }

        /// <summary>
        ///     The root container that contains all services of Maze
        /// </summary>
        private IContainer RootContainer { get; }

        /// <summary>
        ///     The core container, which inherits from <see cref="RootContainer" /> but also provides services from local modules
        /// </summary>
        public ILifetimeScope Container { get; }

        public NuGetFramework Framework { get; } = FrameworkConstants.CommonFrameworks.MazeClient10;
        public NuGetVersion Version { get; } = NuGetVersion.Parse("1.0");

        /// <summary>
        ///     The current synchronization context
        /// </summary>
        public SynchronizationContext Current { get; private set; }

        private ILifetimeScope LoadModules()
        {
            var loader = RootContainer.Resolve<IPackageLockLoader>();

            var modulesConfig = RootContainer.Resolve<IOptions<ModulesOptions>>().Value;
            var lockFile = new FileInfo(Environment.ExpandEnvironmentVariables(modulesConfig.ModulesLockPath));
            if (lockFile.Exists)
            {
                PackagesLock packagesLock;
                try
                {
                    packagesLock = JsonConvert.DeserializeObject<PackagesLock>(File.ReadAllText(lockFile.FullName));

                    var loadContext = loader.Load(packagesLock).Result;
                    if (loadContext.PackagesLoaded) return RootContainer.BeginLifetimeScope(builder => loadContext.Configure(builder));
                }
                catch (Exception)
                {
                    return RootContainer;
                }
            }

            return RootContainer;
        }

        private void StartConnecting()
        {
            Container.Resolve<IManagementCoreConnector>().StartConnecting(Container);
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Split(',').First();
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Split(',').First() == name);
        }

        private void ApplicationOnIdle(object sender, EventArgs e)
        {
            Current = SynchronizationContext.Current;
            SystemEvents.SessionEnding += SystemEventsOnSessionEnding;
        }

        public void Shutdown()
        {
            if (!_shutdownTriggered)
            {
                _shutdownTriggered = true;
                Container.Execute<IShutdownAction, ShutdownContext>(new ShutdownContext(ShutdownTrigger.ApplicationShutdown));
            }

            Application.Exit();
        }

        public void Restart()
        {
            if (!_shutdownTriggered)
            {
                _shutdownTriggered = true;
                Container.Execute<IShutdownAction, ShutdownContext>(new ShutdownContext(ShutdownTrigger.ApplicationRestart));
            }

            Application.Restart();
        }

        private void ApplicationOnApplicationExit(object sender, EventArgs e)
        {
            if (!_shutdownTriggered)
            {
                _shutdownTriggered = true;
                Container.Execute<IShutdownAction, ShutdownContext>(new ShutdownContext(ShutdownTrigger.MainThreadShutdown));
            }

            _notificationIcon.ApplicationExit();
        }

        private void SystemEventsOnSessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (e.Reason != SessionEndReasons.SystemShutdown)
                return;

            if (!_shutdownTriggered)
            {
                _shutdownTriggered = true;
                Container.Execute<IShutdownAction, ShutdownContext>(new ShutdownContext(ShutdownTrigger.SystemShutdown));
            }
        }
    }
}