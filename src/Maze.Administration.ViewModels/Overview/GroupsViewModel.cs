using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Anapher.Wpf.Swan.Extensions;
using MahApps.Metro.IconPacks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Rest.ClientGroups.V1;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.Utilities;
using Maze.Administration.Library.ViewModels;
using Maze.Administration.Library.Views;
using Maze.Administration.ViewModels.Overview.Groups;
using Maze.Server.Connection.Clients;
using Maze.Utilities;
using Prism.Commands;
using Unclassified.TxLib;

namespace Maze.Administration.ViewModels.Overview
{
    public class GroupsViewModel : OverviewTabBase
    {
        private readonly IClientManager _clientManager;
        private readonly IRestClient _restClient;
        private readonly IWindowService _windowService;
        private DelegateCommand _createNewGroupCommand;
        private ListCollectionView _groupsView;
        private string _newGroupName;
        private DelegateCommand<GroupPresenterViewModel> _removeGroupCommand;
        private DelegateCommand<GroupPresenterViewModel> _changeNameCommand;

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

                    ClientGroupsResource.CreateAsync(new ClientGroupDto {Name = NewGroupName}, _restClient).OnErrorShowMessageBox(_windowService)
                        .Forget();
                    NewGroupName = null;
                }));
            }
        }

        public DelegateCommand<GroupPresenterViewModel> RemoveGroupCommand
        {
            get
            {
                return _removeGroupCommand ?? (_removeGroupCommand = new DelegateCommand<GroupPresenterViewModel>(parameter =>
                {
                    if (_windowService.ShowMessage(Tx.T("GroupsView:Warning.SureRemoveGroup", "name", parameter.Group.Name), Tx.T("Warning"),
                            MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.OK)
                        return;

                    ClientGroupsResource.DeleteAsync(parameter.Group.ClientGroupId, _restClient).OnErrorShowMessageBox(_windowService).Forget();
                }));
            }
        }

        public DelegateCommand<GroupPresenterViewModel> ChangeNameCommand
        {
            get
            {
                return _changeNameCommand ?? (_changeNameCommand = new DelegateCommand<GroupPresenterViewModel>(parameter =>
                {
                    _windowService.ShowDialog<ChangeGroupNameViewModel>(vm => vm.Initialize(parameter.Group));
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