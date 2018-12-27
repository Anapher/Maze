using System;
using System.Linq;
using System.Windows.Data;
using MahApps.Metro.IconPacks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Rest.ClientGroups.V1;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.Utilities;
using Orcus.Administration.Library.ViewModels;
using Orcus.Administration.Library.Views;
using Orcus.Administration.ViewModels.Overview.Groups;
using Orcus.Server.Connection.Clients;
using Prism.Commands;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels.Overview
{
    public class GroupsViewModel : OverviewTabBase
    {
        private readonly IClientManager _clientManager;
        private readonly IRestClient _restClient;
        private readonly IWindowService _windowService;
        private DelegateCommand _createNewGroupCommand;
        private ListCollectionView _groupsView;
        private string _newGroupName;

        public GroupsViewModel(IClientManager clientManager, IRestClient restClient, IWindowService windowService) : base(Tx.T("Groups"),
            PackIconFontAwesomeKind.ThLargeSolid)
        {
            _clientManager = clientManager;
            _restClient = restClient;
            _windowService = windowService;
        }

        public ListCollectionView GroupsView
        {
            get => _groupsView;
            set => SetProperty(ref _groupsView, value);
        }

        public string NewGroupName
        {
            get => _newGroupName;
            set => SetProperty(ref _newGroupName, value);
        }

        public DelegateCommand CreateNewGroupCommand
        {
            get
            {
                return _createNewGroupCommand ?? (_createNewGroupCommand = new DelegateCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(NewGroupName))
                        return;

                    if (_clientManager.Groups.Any(x => string.Equals(x.Value.Name, NewGroupName, StringComparison.OrdinalIgnoreCase)))
                    {
                        _windowService.ShowErrorMessageBox(Tx.T("GroupsView:Error.GroupNameExists", "name", NewGroupName));
                        return;
                    }

                    ClientGroupsResource.CreateAsync(new ClientGroupDto {Name = NewGroupName}, _restClient);
                    NewGroupName = null;
                }));
            }
        }

        public override async void OnInitialize()
        {
            await _clientManager.Initialize();

            var groups = new ViewModelObservableCollection<GroupPresenterViewModel, ClientGroupViewModel>(_clientManager.GroupViewModels,
                model => new GroupPresenterViewModel(model));

            GroupsView = new ListCollectionView(groups);
        }
    }
}