using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SystemInformation.Administration.Rest;
using Anapher.Wpf.Toolkit.StatusBar;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.ViewModels;
using Prism.Commands;
using Prism.Regions;
using Unclassified.TxLib;

namespace SystemInformation.Administration.ViewModels
{
    public class SystemInformationViewModel : ViewModelBase
    {
        private readonly ITargetedRestClient _restClient;
        private readonly IShellStatusBar _statusBar;

        private DelegateCommand<SystemInfoViewModel> _copyNameAndValueCommand;
        private DelegateCommand<SystemInfoViewModel> _copyValueCommand;

        private List<SystemInfoGroupViewModel> _groups;

        public SystemInformationViewModel(IShellStatusBar statusBar, ITargetedRestClient restClient)
        {
            _statusBar = statusBar;
            _restClient = restClient;
        }

        public List<SystemInfoGroupViewModel> Groups
        {
            get => _groups;
            private set => SetProperty(ref _groups, value);
        }

        public DelegateCommand<SystemInfoViewModel> CopyNameAndValueCommand
        {
            get
            {
                return _copyNameAndValueCommand ?? (_copyNameAndValueCommand = new DelegateCommand<SystemInfoViewModel>(parameter =>
                {
                    var valueString = parameter.GetValueString();
                    if (string.IsNullOrEmpty(valueString))
                        Clipboard.SetText(parameter.Name);
                    else
                        Clipboard.SetText($"{parameter.Name} = {valueString}");
                }));
            }
        }

        public DelegateCommand<SystemInfoViewModel> CopyValueCommand
        {
            get
            {
                return _copyValueCommand ?? (_copyValueCommand = new DelegateCommand<SystemInfoViewModel>(parameter =>
                {
                    var valueString = parameter.GetValueString();
                    Clipboard.SetText(string.IsNullOrEmpty(valueString) ? parameter.Name : valueString);
                }));
            }
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            var info = await SystemInformationResource.FetchInformation(_restClient)
                .DisplayOnStatusBarCatchErrors(_statusBar, Tx.T("SystemInformation:StatusBar.LoadInformation"));

            if (!info.Failed)
            {
                var groups = info.Result.GroupBy(x => x.Category);
                Groups = groups.Select(x => new SystemInfoGroupViewModel(x.Key, x)).OrderBy(x => x.Childs.Any()).ThenBy(x => x.Name).ToList();
            }
        }
    }
}