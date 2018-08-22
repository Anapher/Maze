using MahApps.Metro.Controls;

namespace Orcus.Administration.Library.Views
{
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