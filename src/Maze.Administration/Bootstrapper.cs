using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Anapher.Wpf.Swan.ViewInterface;
using Autofac;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Maze.Administration.Core.Modules;
using Maze.Administration.Factories;
using Maze.Administration.Library.Menu;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Resources;
using Maze.Administration.Library.Services;
using Maze.Administration.Prism;
using Maze.Administration.Services;
using Maze.Administration.ViewModels;
using Maze.Administration.Views;
using Maze.Administration.Views.Main;
using Prism.Autofac;
using Prism.Modularity;
using Prism.Mvvm;

namespace Maze.Administration
{
    internal class Bootstrapper : AutofacBootstrapper
    {
        private readonly AppLoadContext _appLoadContext;

        public Bootstrapper(AppLoadContext appLoadContext)
        {
            _appLoadContext = appLoadContext;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        protected override DependencyObject CreateShell()
        {
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.InitializePrism();

            return mainWindow;
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog) ModuleCatalog;

            moduleCatalog.AddModule(typeof(ViewModule));

            foreach (var packageCarrier in _appLoadContext.ModulesCatalog.Packages)
            foreach (var type in packageCarrier.Assembly.GetExportedTypes()
                .Where(x => typeof(IModule).IsAssignableFrom(x)))
                moduleCatalog.AddModule(
                    new ModuleInfo(packageCarrier.Context.Package.Id, type.AssemblyQualifiedName)
                    {
                        State = ModuleState.ReadyForInitialization
                    });
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);

            builder.RegisterInstance(_appLoadContext.RestClient).AsImplementedInterfaces();
            builder.RegisterTypeForNavigation<LoginView>();
            builder.RegisterType<AppDispatcher>().As<IAppDispatcher>().SingleInstance();
            builder.RegisterModule<AutofacModule>();
            builder.RegisterType<DefaultMenuFactory>().As<IMenuFactory>().SingleInstance();
            builder.RegisterType<ItemMenuFactory>().As<IItemMenuFactory>().SingleInstance();
            builder.RegisterType<ClientsContextMenu>().SingleInstance();
            builder.RegisterType<OfflineClientsContextMenu>().SingleInstance();
            builder.RegisterInstance(new ViewModelResolver(Assembly.GetAssembly(typeof(MainViewModel)),
                Assembly.GetEntryAssembly())).As<IViewModelResolver>();
            builder.RegisterType<ClientCommandRegistrar>().As<IClientCommandRegistrar>().SingleInstance();
            builder.RegisterType<ShellWindowFactory>().As<IShellWindowFactory>().SingleInstance();
            builder.RegisterInstance(new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>().SingleInstance();
            builder.RegisterType<VisualStudioIcons>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterInstance(Application.Current.MainWindow).As<IWindow>();
            builder.RegisterType<WindowService>().AsImplementedInterfaces().SingleInstance();

            foreach (var packageCarrier in _appLoadContext.ModulesCatalog.Packages)
                builder.RegisterAssemblyModules(packageCarrier.Assembly);
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            
            var resolver = Container.Resolve<IViewModelResolver>();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(resolver.ResolveViewModelType);
        }

        protected override IContainer CreateContainer(ContainerBuilder containerBuilder)
        {
            var container = base.CreateContainer(containerBuilder);
            Initialize(container);

            return container;
        }

        private void Initialize(IContainer container)
        {
            _appLoadContext.RestClient.ServiceProvider = container;

            Mapper.Initialize(config =>
            {
                foreach (var profile in container.Resolve<IEnumerable<Profile>>())
                    config.AddProfile(profile);
            });
        }

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Split(',').First();
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Split(',').First() == name);
        }
    }
}