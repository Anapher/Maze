using System.Windows;
using System.Windows.Media;
using Anapher.Wpf.Swan.ViewInterface;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Library.Services
{
    public interface IShellWindow
    {
        IWindowViewManager ViewManager { get; }
        Window Window { get; }
        void InitializeTitleBar(string title, ImageSource icon);
        void InitializeContent(object content);
        void InitalizeContent(object content, StatusBarManager statusBarManager);
        void Show(IWindow owner);
        bool? ShowDialog(IWindow owner);
    }
}