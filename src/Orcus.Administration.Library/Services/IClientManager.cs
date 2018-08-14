using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Orcus.Administration.Library.Models;

namespace Orcus.Administration.Library.Services
{
    public interface IClientManager
    {
        ObservableCollection<ClientViewModel> ClientViewModels { get; }
        ConcurrentDictionary<int, ClientViewModel> Clients { get; }
        Task Initialize();
    }
}