using System.Windows;
using System.Windows.Media;
using Anapher.Wpf.Swan;
using Anapher.Wpf.Swan.ViewInterface;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Views;
using Orcus.Administration.Views;

namespace Orcus.Administration.Services
{
    public class WindowInstance : IShellWindow
    {
        private readonly ShellWindow _window;

        public WindowInstance(ShellWindow window)
        {
            _window = window;
        }

        public IWindowViewManager ViewManager => _window;
        public Window Window => _window;

        public void InitializeTitleBar(string title, ImageSource icon)
        {
            if (title != null)
                _window.Title = title;

            if (icon != null)
                _window.Icon = icon;
        }

        public void InitializeContent(object content) => InitalizeContent(content, null);

        public void InitalizeContent(object content, StatusBarManager statusBarManager)
        {
            _window.InitializeWindow(content, statusBarManager);
        }

        public void Show(IWindow owner)
        {
            Window.Show();
            Window.CenterOnWindow(owner as Window ?? Application.Current.MainWindow);
        }

        public bool? ShowDialog(Window owner)
        {
            Window.Owner = owner;
            return Window.ShowDialog();
        }

        public bool? ShowDialog(IWindow owner)
        {
            Window.Owner = owner as Window ?? Application.Current.MainWindow;
            return Window.ShowDialog();
        }
    }
}