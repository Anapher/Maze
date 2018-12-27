using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Orcus.Administration.Library.Models;

namespace Orcus.Administration.Library.Services
{
    /// <summary>
    ///     Provides a shared management object for the clients of the server
    /// </summary>
    public interface IClientManager : INotifyPropertyChanged
    {
        ObservableCollection<ClientViewModel> ClientViewModels { get; }
        ConcurrentDictionary<int, ClientViewModel> Clients { get; }

        ObservableCollection<ClientGroupViewModel> GroupViewModels { get; }
        ConcurrentDictionary<int, ClientGroupViewModel> Groups { get; }

        /// <summary>
        ///     Make sure to call <see cref="Initialize" /> once before using the members of this class.
        /// </summary>
        Task Initialize();
    }
}