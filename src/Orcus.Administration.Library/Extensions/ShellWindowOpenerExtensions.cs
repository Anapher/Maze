using System.Windows;
using System.Windows.Media;
using Orcus.Administration.Library.Services;

namespace Orcus.Administration.Library.Extensions
{
    public static class ShellWindowOpenerExtensions
    {
        public static Window Show(this IShellWindowOpener shellWindowOpener, FrameworkElement view) =>
            shellWindowOpener.Show(view, null, null, null);

        public static Window Show(this IShellWindowOpener shellWindowOpener, FrameworkElement view, string title) =>
            shellWindowOpener.Show(view, title, null, null);

        public static Window Show(this IShellWindowOpener shellWindowOpener, FrameworkElement view, string title, ImageSource icon) =>
            shellWindowOpener.Show(view, title, icon, null);
    }
}