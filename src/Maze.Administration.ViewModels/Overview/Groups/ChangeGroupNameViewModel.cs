using System;
using System.Linq;
using Anapher.Wpf.Swan;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Rest.ClientGroups.V1;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.Views;
using Maze.Server.Connection.Clients;
using Prism.Commands;
using Prism.Mvvm;
using Unclassified.TxLib;

namespace Maze.Administration.ViewModels.Overview.Groups
{
    public class ChangeGroupNameViewModel : BindableBase
    {
        private readonly IClientManager _clientManager;
        private readonly IRestClient _restClient;
        private readonly IWindowService _windowService;
        private DelegateCommand _cancelCommand;
        private ClientGroupViewModel _clientGroupViewModel;
        private bool? _dialogResult;
        private string _newGroupName;
        private AsyncRelayCommand _okCommand;

        public ChangeGroupNameViewModel(IClientManager clientManager, IWindowService windowService, IRestClient restClient)
        {
            _clientManager = clientManager;
            _windowService = windowService;
            _restClient = restClient;
        }

        public string NewGroupName
        {
            get => _newGroupName;
            set => SetProperty(ref _newGroupName, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public AsyncRelayCommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new AsyncRelayCommand(async parameter =>
                {
                    if (string.IsNullOrWhiteSpace(NewGroupName))
                        return;

                    if (_clientManager.Groups.Any(x => string.Equals(x.Value.Name, NewGroupName, StringComparison.OrdinalIgnoreCase)))
                    {
                        _windowService.ShowErrorMessageBox(Tx.T("GroupsView:Error.GroupNameExists", "name", NewGroupName));
                        return;
                    }

                    if (NewGroupName == _clientGroupViewModel.Name)
                    {
                        DialogResult = true;
                        return;
                    }

                    if (await ClientGroupsResource
                        .PutGroup(new ClientGroupDto {ClientGroupId = _clientGroupViewModel.ClientGroupId, Name = NewGroupName}, _restClient)
                        .OnErrorShowMessageBox(_windowService))
                        DialogResult = true;
                }));
            }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new DelegateCommand(() => { DialogResult = false; })); }
        }

        public void Initialize(ClientGroupViewModel clientGroupViewModel)
        {
            _clientGroupViewModel = clientGroupViewModel;
            NewGroupName = clientGroupViewModel.Name;
        }
    }
}