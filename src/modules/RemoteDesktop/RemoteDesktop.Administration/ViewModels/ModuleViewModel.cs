using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;

namespace RemoteDesktop.Administration.ViewModels
{
    public class ModuleViewModel : ViewModelBase
    {
        private readonly IShellStatusBar _statusBar;
        private readonly IPackageRestClient _restClient;

        public ModuleViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient.CreatePackageSpecific("Module");
        }
    }
}