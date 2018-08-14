using System.Linq;
using System.Reflection;
using System.Windows;
using Autofac;
using Orcus.Administration.Core.Modules;
using Orcus.Administration.Prism;
using Orcus.Administration.ViewModels;
using Orcus.Administration.ViewModels.Main;
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
                moduleCatalog.AddModule(type);
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);

            builder.RegisterInstance(_appLoadContext.RestClient);
            builder.RegisterTypeForNavigation<LoginView>();

            foreach (var packageCarrier in _appLoadContext.ModulesCatalog.Packages)
                builder.RegisterAssemblyModules(packageCarrier.Assembly);
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            var resolver = new ViewModelResolver(Assembly.GetAssembly(typeof(MainViewModel)));
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(resolver.ResolveViewModelType);
        }
    }
}