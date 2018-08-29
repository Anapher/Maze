using System.Windows.Media;
using MahApps.Metro.Controls;

namespace Orcus.Administration.Library.Views
{
    public interface IWindowViewManager : IDialogWindow
    {
        string Title { get; set; }
        object RightStatusBarContent { get; set; }
        bool EscapeClosesWindow { get; set; }
        bool? DialogResult { get; set; }
        bool ShowInTaskbar { get; set; }
        WindowCommands LeftWindowCommands { get; set; }
        WindowCommands RightWindowCommands { get; set; }
        object TitleBarIcon { get; set; }
        ImageSource TaskBarIcon { get; set; }
        FlyoutsControl Flyouts { get; set; }
    }
}