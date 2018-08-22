using Anapher.Wpf.Swan.ViewInterface;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace Orcus.Administration.Library.Views
{
    public interface IMetroWindow : IWindow
    {
        bool? ShowDialog(VistaFileDialog fileDialog);
        bool? ShowDialog(FileDialog fileDialog);
    }
}