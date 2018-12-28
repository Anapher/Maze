using FileExplorer.Administration.Utilities;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.StatusBar;
using Maze.Administration.Library.Views;

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