using System.Reflection;
using System.Windows;
using Autofac;
using Orcus.Administration.Prism;
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
        protected override DependencyObject CreateShell() => Container.Resolve<MainWindow>();

        protected override void InitializeShell()
        {
            // ReSharper disable once PossibleNullReferenceException
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog) ModuleCatalog;

            moduleCatalog.AddModule(typeof(ViewModule));
            moduleCatalog.AddModule(typeof(ViewModelModule));
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);
            builder.RegisterTypeForNavigation<LoginView>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            var resolver = new ViewModelResolver(Assembly.GetAssembly(typeof(MainViewModel)));
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(resolver.ResolveViewModelType);
        }
    }
}