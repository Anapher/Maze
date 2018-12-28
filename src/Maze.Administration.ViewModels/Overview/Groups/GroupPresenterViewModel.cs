using System;
using System.Collections;
using System.Windows.Data;
using Maze.Administration.Library.Models;
using Prism.Mvvm;

namespace Maze.Administration.ViewModels.Overview.Groups
{
    public class GroupPresenterViewModel : BindableBase
    {
        public GroupPresenterViewModel(ClientGroupViewModel group)
        {
            Group = group;

            OnlineClients = new ListCollectionView((IList) group.Clients);
            OnlineClients.LiveFilteringProperties.Add(nameof(ClientViewModel.IsSocketConnected));
            OnlineClients.IsLiveFiltering = true;
            OnlineClients.Filter = OnlineClientFilter;
            OnlineClients.Refresh();

            ActiveClients24H = new ListCollectionView((IList) group.Clients);
            ActiveClients24H.LiveFilteringProperties.Add(nameof(ClientViewModel.IsSocketConnected));
            ActiveClients24H.IsLiveFiltering = true;
            ActiveClients24H.Filter = ActiveClientFilter;
            ActiveClients24H.Refresh();

            ActiveClients7Days = new ListCollectionView((IList)group.Clients);
            ActiveClients7Days.LiveFilteringProperties.Add(nameof(ClientViewModel.IsSocketConnected));
            ActiveClients7Days.IsLiveFiltering = true;
            ActiveClients7Days.Filter = ActiveClients7DaysFilter;
            ActiveClients7Days.Refresh();
        }

        public ClientGroupViewModel Group { get; }
        public ListCollectionView OnlineClients { get; }
        public ListCollectionView ActiveClients24H { get; }
        public ListCollectionView ActiveClients7Days { get; }

        private bool OnlineClientFilter(object obj)
        {
            var clientVm = (ClientViewModel)obj;
            return clientVm.IsSocketConnected;
        }

        private bool ActiveClientFilter(object obj)
        {
            var clientVm = (ClientViewModel)obj;
            return clientVm.LatestSession?.CreatedOn < DateTimeOffset.Now.AddDays(-1);
        }

        private bool ActiveClients7DaysFilter(object obj)
        {
            var clientVm = (ClientViewModel)obj;
            return clientVm.LatestSession?.CreatedOn < DateTimeOffset.Now.AddDays(-7);
        }
    }
}