using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.Windows;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Rest.ClientGroups.V1;
using Maze.Utilities;
using Prism.Commands;
using Unclassified.TxLib;

namespace Maze.Administration.ViewModels.Overview.Groups
{
    public class GroupMenuItemViewModel
    {
        private readonly IWindowService _windowService;
        private readonly IRestClient _restClient;
        private DelegateCommand<object[]> _toggleAddCommand;

        public GroupMenuItemViewModel(IWindowService windowService, IRestClient restClient)
        {
            _windowService = windowService;
            _restClient = restClient;
        }

        public DelegateCommand<object[]> ToggleAddCommand
        {
            get
            {
                return _toggleAddCommand ?? (_toggleAddCommand = new DelegateCommand<object[]>(parameter =>
                {
                    var groupViewModel = (ClientGroupViewModel) parameter[0];
                    var selectedItems = (IList) parameter[1];

                    var clientsInGroup = new List<ClientViewModel>();
                    var clientsNotInGroup = new List<ClientViewModel>();

                    foreach (var clientViewModel in selectedItems.Cast<ClientViewModel>())
                    {
                        if (groupViewModel.Clients.Contains(clientViewModel))
                            clientsInGroup.Add(clientViewModel);
                        else clientsNotInGroup.Add(clientViewModel);
                    }

                    if (!clientsNotInGroup.Any())
                    {
                        ClientGroupsResource.DeleteClientsFromGroup(groupViewModel.ClientGroupId, clientsInGroup.Select(x => x.ClientId), _restClient)
                            .OnErrorShowMessageBox(_windowService).Forget();
                    }
                    else
                    {
                        if (clientsInGroup.Any())
                        {
                            if (_windowService.ShowMessage(Tx.T("GroupsView:Warning.ClientsAreAlreadyInGroup"), Tx.T("Warning"),
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes)
                                return;
                        }

                        ClientGroupsResource.PostClientsToGroup(groupViewModel.ClientGroupId, clientsNotInGroup.Select(x => x.ClientId), _restClient)
                            .OnErrorShowMessageBox(_windowService).Forget();
                    }
                }));
            }
        }
    }
}