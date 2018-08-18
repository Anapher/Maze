using Anapher.Wpf.Swan.ViewInterface;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace Orcus.Administration.Library.Views
{
    public interface IMetroWindow : IWindow
    {
        void AddFlyout(Flyout flyout);
        bool? ShowDialog(VistaFileDialog fileDialog);
        bool? ShowDialog(FileDialog fileDialog);
    }
}