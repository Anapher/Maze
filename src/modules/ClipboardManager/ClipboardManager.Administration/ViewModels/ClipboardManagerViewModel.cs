using Maze.Administration.Library.Clients;
using Maze.Administration.Library.StatusBar;
using Maze.Administration.Library.ViewModels;

namespace ClipboardManager.Administration.ViewModels
{
    public class ClipboardManagerViewModel : ViewModelBase
    {
        private readonly IShellStatusBar _statusBar;
        private readonly ITargetedRestClient _restClient;

        public ClipboardManagerViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient;
        }
    }
}