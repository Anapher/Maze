using System.Collections.Generic;
using System.Linq;
using SystemInformation.Administration.Rest;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.ViewModels;
using Prism.Regions;
using Unclassified.TxLib;

namespace SystemInformation.Administration.ViewModels
{
    public class SystemInformationViewModel : ViewModelBase
    {
        private readonly IPackageRestClient _restClient;
        private readonly IShellStatusBar _statusBar;

        private List<SystemInfoGroupViewModel> _groups;

        public SystemInformationViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient.CreatePackageSpecific("SystemInformation");
        }

        public List<SystemInfoGroupViewModel> Groups
        {
            get => _groups;
            private set => SetProperty(ref _groups, value);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            var info = await SystemInformationResource.FetchInformation(_restClient)
                .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("SystemInformation:StatusBar.LoadInformation"));

            if (!info.Failed)
            {
                var groups = info.Result.GroupBy(x => x.Category);
                Groups = groups.Select(x => new SystemInfoGroupViewModel(x.Key, x)).ToList();
            }
        }
    }
}