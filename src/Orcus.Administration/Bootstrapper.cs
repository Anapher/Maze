using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Orcus.Administration.Core.Modules;
using Orcus.Administration.Factories;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Resources;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Prism;
using Orcus.Administration.Services;
using Orcus.Administration.ViewModels;
using Orcus.Administration.Views;
using Orcus.Administration.Views.Main;
using Prism.Autofac;
using Prism.Modularity;
using Prism.Mvvm;

namespace Orcus.Administration
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

            builder.RegisterInstance(_appLoadContext.RestClient);
            builder.RegisterTypeForNavigation<LoginView>();
            builder.RegisterType<AppDispatcher>().As<IAppDispatcher>().SingleInstance();
            builder.RegisterModule<AutofacModule>();
            builder.RegisterType<DefaultMenuFactory>().As<IMenuFactory>().SingleInstance();
            builder.RegisterType<ItemMenuFactory>().As<IItemMenuFactory>().SingleInstance();
            builder.RegisterType<ClientsContextMenu>().SingleInstance();
            builder.RegisterInstance(new ViewModelResolver(Assembly.GetAssembly(typeof(MainViewModel)),
                Assembly.GetEntryAssembly())).As<IViewModelResolver>();
            builder.RegisterType<ClientCommandRegistrar>().As<IClientCommandRegistrar>().SingleInstance();
            builder.RegisterType<ShellWindowFactory>().As<IShellWindowFactory>().SingleInstance();
            builder.RegisterInstance(new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>().SingleInstance();
            builder.RegisterType<VisualStudioIcons>().AsImplementedInterfaces().SingleInstance();
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

        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Split(',').First();
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Split(',').First() == name);
        }
    }
}