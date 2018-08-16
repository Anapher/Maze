using System.Windows;
using System.Windows.Media;

namespace Orcus.Administration.Library.Services
{
    public interface IShellWindowOpener
    {
        Window Show(FrameworkElement view, string title, ImageSource icon);
    }
}