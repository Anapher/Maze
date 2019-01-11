using Anapher.Wpf.Toolkit.StatusBar;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.ViewModels;

namespace ModuleTemplate.Administration.ViewModels
{
    public class ModuleViewModel : ViewModelBase
    {
        private readonly IShellStatusBar _statusBar;
        private readonly ITargetedRestClient _restClient;

        public ModuleViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient;
        }
    }
}