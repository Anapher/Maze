using System.Windows;
using System.Windows.Media;
using Anapher.Wpf.Swan.ViewInterface;
using MahApps.Metro.Controls;
using Orcus.Administration.Library.StatusBar;

namespace Orcus.Administration.Library.Views
{
    public interface IShellWindow : IWindow
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
        ResizeMode ResizeMode { get; set; }
        double Height { get; set; }
        double Width { get; set; }
        SizeToContent SizeToContent { get; set; }

        void InitalizeContent(object content, StatusBarManager statusBarManager);
        void Show(IWindow owner);
        bool? ShowDialog(IWindow owner);
    }
}