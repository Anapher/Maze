using System.Windows;
using System.Windows.Media;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Library.Extensions
{
    public static class ShellWindowFactoryExtensions
    {
        public static IMetroWindow Show(this IShellWindowFactory shellWindowFactory, FrameworkElement view)
        {
            var window = shellWindowFactory.Create();
            window.InitalizeContent(view, null);
            window.Show();

            return window.ViewManager;
        }

        public static IMetroWindow Show(this IShellWindowFactory shellWindowFactory, FrameworkElement view,
            string title)
        {
            var window = shellWindowFactory.Create();
            window.InitializeTitleBar(title, null);
            window.InitalizeContent(view, null);
            window.Show();

            return window.ViewManager;
        }

        public static IMetroWindow Show(this IShellWindowFactory shellWindowFactory, FrameworkElement view,
            string title, ImageSource icon)
        {
            var window = shellWindowFactory.Create();
            window.InitializeTitleBar(title, icon);
            window.InitalizeContent(view, null);
            window.Show();

            return window.ViewManager;
        }
    }
}