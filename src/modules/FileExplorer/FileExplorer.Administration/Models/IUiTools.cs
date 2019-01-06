using Anapher.Wpf.Toolkit.StatusBar;
using Anapher.Wpf.Toolkit.Windows;
using FileExplorer.Administration.Utilities;
using Maze.Administration.Library.Services;

namespace FileExplorer.Administration.Models
{
    public interface IUiTools
    {
        IShellStatusBar StatusBar { get; }
        IWindowService Window { get; }
        IImageProvider ImageProvider { get; }
        IAppDispatcher Dispatcher { get; }
    }
}