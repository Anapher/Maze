using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Rest.ClientGroups.V1;
using Maze.Administration.Library.Rest.Clients.V1;
using Maze.Administration.Library.Services;
using Maze.Server.Connection;
using Maze.Server.Connection.Clients;

namespace Maze.Administration.Core.Clients
{
    public class ClientManager : IClientManager, INotifyPropertyChanged
    {
        private readonly IAppDispatcher _appDispatcher;
        private readonly SemaphoreSlim _initalizationLock = new SemaphoreSlim(1, 1);
        private readonly IMazeRestClient _restClient;
        private bool _isInitialized;

        public ClientManager(IMazeRestClient restClient, IAppDispatcher appDispatcher)
        {
            _restClient = restClient;
            _appDispatcher = appDispatcher;
        }

        public ObservableCollection<ClientViewModel> ClientViewModels { get; private set; }
        public ConcurrentDictionary<int, ClientViewModel> Clients { get; private set; }

        public ObservableCollection<ClientGroupViewModel> GroupViewModels { get; private set; } 
        public ConcurrentDictionary<int, ClientGroupViewModel> Groups { get; private set; }

        public async Task Initialize()
        {
            if (_isInitialized)
                return;

            await _initalizationLock.WaitAsync();
            try
            {
                if (_isInitialized)
                    return;

                var groupsTask = ClientGroupsResource.GetGroups(_restClient);

                var allClients = await ClientsResource.FetchAsync(_restClient);
                _restClient.HubConnection.On<ClientDto>(HubEventNames.ClientConnected, OnClientConnected);
                _restClient.HubConnection.On<int>(HubEventNames.ClientDisconnected, OnClientDisconnected);

                var viewModels = await _appDispatcher.Current.InvokeAsync(() =>
                    new ObservableCollection<ClientViewModel>(allClients.Select(dto => new DefaultClientViewModel(dto))));

                Clients = new ConcurrentDictionary<int, ClientViewModel>(viewModels.ToDictionary(x => x.ClientId, x => x));
                ClientViewModels = viewModels;

                var groups = await groupsTask;
                _restClient.HubConnection.On<ClientGroupDto>(HubEventNames.ClientGroupCreated, OnClientGroupCreated);
                _restClient.HubConnection.On<ClientGroupDto>(HubEventNames.ClientGroupUpdated, OnClientGroupUpdated);
                _restClient.HubConnection.On<int>(HubEventNames.ClientGroupRemoved, OnClientGroupRemoved);

                var groupViewModels = await _appDispatcher.Current.InvokeAsync(() =>
                    new ObservableCollection<ClientGroupViewModel>(groups.Select(x => new DefaultClientGroupViewModel(x, Clients))));
                Groups = new ConcurrentDictionary<int, ClientGroupViewModel>(groupViewModels.ToDictionary(x => x.ClientGroupId, x => x));
                GroupViewModels = groupViewModels;

                _isInitialized = true;
            }
            finally
            {
                _initalizationLock.Release();
            }

            var foo = _appDispatcher.Current.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                OnPropertyChanged(nameof(Clients));
                OnPropertyChanged(nameof(ClientViewModels));
                OnPropertyChanged(nameof(Groups));
                OnPropertyChanged(nameof(GroupViewModels));
            }));
        }

        private void OnClientGroupUpdated(ClientGroupDto obj)
        {
            if (Groups.TryGetValue(obj.ClientGroupId, out var groupViewModel))
            {
                UpdateGroupViewModel(groupViewModel, obj);
            }
        }

        private void OnClientGroupRemoved(int obj)
        {
            if (Groups.TryRemove(obj, out var groupViewModel))
            {
                _appDispatcher.Current.BeginInvoke(DispatcherPriority.Background, (Action) (() => GroupViewModels.Remove(groupViewModel)));
            }
        }

        private void OnClientGroupCreated(ClientGroupDto obj)
        {
            var groupViewModel = new DefaultClientGroupViewModel(obj, Clients);
            if (Groups.TryAdd(groupViewModel.ClientGroupId, groupViewModel))
            {
                _appDispatcher.Current.BeginInvoke(DispatcherPriority.Background, (Action) (() => GroupViewModels.Add(groupViewModel)));
            }
        }

        private void OnClientDisconnected(int clientId)
        {
            if (Clients.TryGetValue(clientId, out var clientViewModel))
                clientViewModel.IsSocketConnected = false;
        }

        private void OnClientConnected(ClientDto clientDto)
        {
            if (Clients.TryGetValue(clientDto.ClientId, out var clientViewModel))
            {
                UpdateClientViewModel(clientViewModel, clientDto);
            }
            else
            {
                clientViewModel = new DefaultClientViewModel(clientDto);
                if (Clients.TryAdd(clientDto.ClientId, clientViewModel))
                    _appDispatcher.Current.BeginInvoke(DispatcherPriority.Background, (Action) (() => ClientViewModels.Add(clientViewModel)));
            }
        }

        private static void UpdateClientViewModel(ClientViewModel clientViewModel, ClientDto clientDto)
        {
            clientViewModel.Username = clientDto.Username;
            clientViewModel.OperatingSystem = clientDto.OperatingSystem;
            clientViewModel.MacAddress = clientDto.MacAddress;
            clientViewModel.SystemLanguage = clientDto.SystemLanguage;
            clientViewModel.HardwareId = clientDto.HardwareId;
            clientViewModel.CreatedOn = clientDto.CreatedOn;
            clientViewModel.IsSocketConnected = clientDto.IsSocketConnected;
            clientViewModel.LatestSession = clientDto.LatestSession;
        }

        private void UpdateGroupViewModel(ClientGroupViewModel groupViewModel, ClientGroupDto groupDto)
        {
            groupViewModel.Name = groupDto.Name;
            if (groupDto.Clients != null)
            {
                _appDispatcher.Current.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    var toRemove = groupViewModel.Clients.ToList();
                    foreach (var clientId in groupDto.Clients)
                    {
                        var existingClient = groupViewModel.Clients.FirstOrDefault(x => x.ClientId == clientId);
                        if (existingClient != null)
                            toRemove.Remove(existingClient);
                        else
                            groupViewModel.Clients.Add(Clients[clientId]);
                    }

                    foreach (var clientViewModel in toRemove)
                        groupViewModel.Clients.Remove(clientViewModel);
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}