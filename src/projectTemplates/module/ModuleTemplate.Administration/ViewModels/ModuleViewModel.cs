using Anapher.Wpf.Toolkit.StatusBar;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.ViewModels;

namespace ModuleTemplate.Administration.ViewModels
{
    public class ModuleNamePlaceholderViewModel : ViewModelBase
    {
        private readonly IShellStatusBar _statusBar;
        private readonly ITargetedRestClient _restClient;

        public ModuleNamePlaceholderViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient;
        }
    }
}