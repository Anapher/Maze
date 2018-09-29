using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;

namespace ClipboardManager.Administration.ViewModels
{
    public class ClipboardManagerViewModel : ViewModelBase
    {
        private readonly IShellStatusBar _statusBar;
        private readonly IPackageRestClient _restClient;

        public ClipboardManagerViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient.CreatePackageSpecific("ClipboardManager");
        }
    }
}