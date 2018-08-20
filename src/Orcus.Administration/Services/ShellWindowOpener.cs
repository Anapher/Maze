using System.Windows;
using System.Windows.Media;
using Anapher.Wpf.Swan;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Views;

namespace Orcus.Administration.Services
{
    public class ShellWindowOpener : IShellWindowOpener
    {
        public Window Show(FrameworkElement view, string title, ImageSource icon, IShellStatusBar statusBar)
        {
            var shellWindow = new ShellWindow(view);
            if (!string.IsNullOrEmpty(title))
                shellWindow.Title = title;

            if (icon != null)
                shellWindow.Icon = icon;

            shellWindow.Show();
            shellWindow.CenterOnWindow(Application.Current.MainWindow);

            return shellWindow;
        }
    }
}