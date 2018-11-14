using System.Security;
using Anapher.Wpf.Swan.ViewInterface;
using Orcus.Administration.Core.Modules;
using Orcus.Administration.ViewModels;
using Orcus.Administration.ViewModels.Main;
using Orcus.Administration.Views.Main;
using Prism.Mvvm;
using Prism.Regions;

namespace Orcus.Administration.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var module = new PrismModule();
            module.Initialize();

            DataContext = new LoginViewModel(LoadAppAction);
            MainContentControl.Content = new LoginView();

#if DEBUG
            var loginVm = (LoginViewModel) DataContext;
            loginVm.Username = "test";
            var pw = new SecureString();
            foreach (var c in "test")
                pw.AppendChar(c);

            loginVm.LoginCommand.Execute(pw);
#endif
        }

        private void LoadAppAction(AppLoadContext context)
        {
            MainContentControl.Content = null;

            var bootstrapper = new Bootstrapper(context);
            bootstrapper.Run();
        }

        public void InitializePrism()
        {
            ViewModelLocator.SetAutoWireViewModel(this, true);
            RegionManager.SetRegionName(MainContentControl, "MainContent");
        }
    }
}