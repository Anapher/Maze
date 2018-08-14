using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Orcus.Administration.Core.Clients;
using Orcus.Administration.Core.Rest.Clients.V1;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Orcus.Server.Connection.Clients;

namespace Orcus.Administration.ViewModels.Overview.Clients
{
    public class ClientManager : IClientManager
    {
        private readonly IOrcusRestClient _restClient;
        private readonly IAppDispatcher _appDispatcher;
        private readonly SemaphoreSlim _initalizationLock = new SemaphoreSlim(1, 1);
        private bool _isInitialized;

        public ClientManager(IOrcusRestClient restClient, IAppDispatcher appDispatcher)
        {
            _restClient = restClient;
            _appDispatcher = appDispatcher;
        }

        public ObservableCollection<ClientViewModel> ClientViewModels { get; private set; }
        public ConcurrentDictionary<int, ClientViewModel> Clients { get; private set; }

        public async Task Initialize()
        {
            if (_isInitialized)
                return;
            
            await _initalizationLock.WaitAsync();
            try
            {
                if (_isInitialized)
                    return;

                var allClients = await ClientsResource.FetchAsync(_restClient);
                _restClient.HubConnection.On<ClientDto>("ClientConnected", OnClientConnected);
                _restClient.HubConnection.On<int>("ClientDisconnected", OnClientDisconnected);

                var viewModels = await _appDispatcher.Current.InvokeAsync(() =>
                    new ObservableCollection<ClientViewModel>(allClients.Select(dto => new ClientViewModel(dto))));

                Clients = new ConcurrentDictionary<int, ClientViewModel>(viewModels.ToDictionary(x => x.ClientId, x => x));
                ClientViewModels = viewModels;

                _isInitialized = true;
            }
            finally
            {
                _initalizationLock.Release();
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
                clientViewModel.Update(clientDto);
            }
            else
            {
                clientViewModel = new ClientViewModel(clientDto);
                if (Clients.TryAdd(clientDto.ClientId, clientViewModel))
                {
                    _appDispatcher.Current.BeginInvoke(DispatcherPriority.Background,
                        (Action) (() => ClientViewModels.Add(clientViewModel)));
                }
            }
        }
    }
}