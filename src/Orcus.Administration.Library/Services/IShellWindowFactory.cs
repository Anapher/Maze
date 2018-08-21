using System.Windows;
using System.Windows.Media;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Library.Services
{
    public interface IShellWindowFactory
    {
        IShellWindow Create();
    }

    public interface IShellWindow
    {
        IWindowViewManager ViewManager { get; }
        Window Window { get; }
        void InitializeTitleBar(string title, ImageSource icon);
        void InitializeContent(object content);
        void InitalizeContent(object content, StatusBarManager statusBarManager);
        void Show();
    }
}