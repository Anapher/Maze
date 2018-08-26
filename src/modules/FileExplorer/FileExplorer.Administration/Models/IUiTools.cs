using Anapher.Wpf.Swan.ViewInterface;
using FileExplorer.Administration.Utilities;
using Orcus.Administration.Library.StatusBar;

namespace FileExplorer.Administration.Models
{
    public interface IUiTools
    {
        IShellStatusBar StatusBar { get; }
        IWindow Window { get; }
        IImageProvider ImageProvider { get; }
    }
}