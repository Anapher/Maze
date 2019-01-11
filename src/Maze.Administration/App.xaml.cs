using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml;
using Anapher.Wpf.Toolkit.Metro.Extensions;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Maze.Administration.ViewModels;
using Maze.Administration.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;

namespace Maze.Administration
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var stream = GetResourceStream(new Uri("/Resources/SyntaxHighlighting/Json.xshd", UriKind.Relative)).Stream)
            using (var reader = new XmlTextReader(stream))
            {
                HighlightingManager.Instance.RegisterHighlighting("Json", new string[0],
                    HighlightingLoader.Load(reader, HighlightingManager.Instance));
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterWindowServices(Assembly.GetEntryAssembly(), typeof(MainViewModel).Assembly);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

            moduleCatalog.AddModule<PrismModule>();
            moduleCatalog.AddModule<ViewModule>();
        }

        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);

            Container.GetContainer().RegisterShell(shell);
            ViewModelLocator.SetAutoWireViewModel(shell, true);
        }

        protected override Window CreateShell() => new MainWindow();

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Split(',').First();
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Split(',').First() == name);
        }
    }
}