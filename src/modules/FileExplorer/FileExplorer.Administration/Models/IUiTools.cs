using FileExplorer.Administration.Utilities;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Views;

namespace FileExplorer.Administration.Models
{
    public interface IUiTools
    {
        IShellStatusBar StatusBar { get; }
        IDialogWindow Window { get; }
        IImageProvider ImageProvider { get; }
        IAppDispatcher Dispatcher { get; }
    }
}