using Anapher.Wpf.Swan.ViewInterface;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace Orcus.Administration.Library.Views
{
    public interface IMetroWindow : IWindow
    {
        bool? ShowDialog(VistaFileDialog fileDialog);
        bool? ShowDialog(FileDialog fileDialog);
    }

    public interface IWindowViewManager : IMetroWindow
    {
        string Title { get; set; }
        object RightStatusBarContent { get; set; }
        bool EscapeClosesWindow { get; set; }
        WindowCommands LeftWindowCommands { get; set; }
        WindowCommands RightWindowCommands { get; set; }
        object TitleBarIcon { get; set; }
        FlyoutsControl Flyouts { get; set; }
    }
}