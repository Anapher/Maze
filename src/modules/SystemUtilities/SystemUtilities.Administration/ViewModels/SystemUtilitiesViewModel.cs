using Maze.Administration.Library.Clients;
using Maze.Administration.Library.StatusBar;
using Maze.Administration.Library.ViewModels;

namespace SystemUtilities.Administration.ViewModels
{
    public class SystemUtilitiesViewModel : ViewModelBase
    {
        private readonly ITargetedRestClient _restClient;
        private readonly IShellStatusBar _statusBar;

        public SystemUtilitiesViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient;
        }
    }
}