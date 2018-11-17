using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;

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