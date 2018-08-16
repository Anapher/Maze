using System.Windows;
using Orcus.Administration.Library.Services;

namespace Orcus.Administration.Library.Extensions
{
    public static class ShellWindowOpenerExtensions
    {
        public static Window Show(this IShellWindowOpener shellWindowOpener, FrameworkElement view) =>
            shellWindowOpener.Show(view, null, null);

        public static Window Show(this IShellWindowOpener shellWindowOpener, FrameworkElement view, string title) =>
            shellWindowOpener.Show(view, title, null);
    }
}