using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Anapher.Wpf.Swan;
using Anapher.Wpf.Swan.ViewInterface;
using Orcus.Administration.ViewModels;

namespace Orcus.Administration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var windowServiceInterface = InitializeWindowServiceInterface();

            new MainWindow().Show();
            windowServiceInterface.RegisterMainWindow();
        }

        private static WpfWindowServiceInterface InitializeWindowServiceInterface()
        {
            var windowServiceInterface = new WpfWindowServiceInterface();
            windowServiceInterface.RegisterWindow<MainWindow, MainViewModel>();


            WindowServiceInterface.Initialize(windowServiceInterface);
            return windowServiceInterface;
        }
    }
}
