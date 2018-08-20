using System.Windows;
using System.Windows.Media;
using Orcus.Administration.Library.StatusBar;

namespace Orcus.Administration.Library.Services
{
    public interface IShellWindowOpener
    {
        Window Show(FrameworkElement view, string title, ImageSource icon, IShellStatusBar statusBar);
    }
}