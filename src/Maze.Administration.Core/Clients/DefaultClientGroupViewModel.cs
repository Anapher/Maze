using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Maze.Administration.Library.Models;
using Maze.Server.Connection.Clients;

namespace Maze.Administration.Core.Clients
{
    public class DefaultClientGroupViewModel : ClientGroupViewModel
    {
        private string _name;

        public DefaultClientGroupViewModel(ClientGroupDto clientGroupDto, IDictionary<int, ClientViewModel> clients)
        {
            ClientGroupId = clientGroupDto.ClientGroupId;
            Name = clientGroupDto.Name;

            var clientsCollection =
                new ObservableCollection<ClientViewModel>(clientGroupDto.Clients?.Select(x =>
                {
                    var client = clients[x];
                    client.Groups.Add(this);
                    return client;
                }) ?? Enumerable.Empty<ClientViewModel>());
            clientsCollection.CollectionChanged += ClientsCollectionOnCollectionChanged;

            Clients = clientsCollection;
        }

        public override int ClientGroupId { get; }

        public override string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public override IList<ClientViewModel> Clients { get; }

        private void ClientsCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var clientViewModel in e.NewItems.Cast<ClientViewModel>())
                {
                    clientViewModel.Groups.Add(this);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var clientViewModel in e.OldItems.Cast<ClientViewModel>())
                {
                    clientViewModel.Groups.Remove(this);
                }
            }
        }
    }
}