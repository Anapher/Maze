using System;
using System.Threading.Tasks;
using System.Windows.Data;
using MahApps.Metro.IconPacks;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.ViewModels;

namespace Maze.Administration.ViewModels.Overview.Clients
{
    public class DefaultClientListViewModel : ClientListBase
    {
        private readonly IClientManager _clientManager;
        private ListCollectionView _clientsView;

        public DefaultClientListViewModel(IClientManager clientManager) : base("Default List", PackIconFontAwesomeKind.BarsSolid)
        {
            _clientManager = clientManager;
            Load();
        }

        public ListCollectionView ClientsView
        {
            get => _clientsView;
            private set => SetProperty(ref _clientsView, value);
        }

        private async Task Load()
        {
            await _clientManager.Initialize();
            ClientsView = new ListCollectionView(_clientManager.ClientViewModels);
            ClientsView.Filter = Filter;
        }

        private bool Filter(object obj)
        {
            if (string.IsNullOrEmpty(SearchText))
                return true;

            var clientViewModel = (ClientViewModel) obj;
            return clientViewModel.Username.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1 ||
                   clientViewModel.OperatingSystem.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1 ||
                   clientViewModel.MacAddress.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1;
        }

        protected override void OnSearchTextChanged(string searchText)
        {
            base.OnSearchTextChanged(searchText);

            ClientsView?.Refresh();
        }
    }
}