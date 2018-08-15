using System.Threading.Tasks;
using System.Windows.Data;
using MahApps.Metro.IconPacks;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.ViewModels;

namespace Orcus.Administration.ViewModels.Overview.Clients
{
    public class DefaultClientListViewModel : ClientListBase
    {
        private readonly IClientManager _clientManager;
        private ListCollectionView _clientsView;

        public DefaultClientListViewModel(IClientManager clientManager) : base("Default List",
            PackIconFontAwesomeKind.BarsSolid)
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
        }
    }
}