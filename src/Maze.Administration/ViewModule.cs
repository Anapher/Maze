using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Maze.Administration.Library;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.Views;
using Maze.Administration.ViewModels.Overview.Groups;
using Maze.Administration.Views.Main;
using Maze.Administration.Views.Main.Overview;
using Maze.Administration.Views.Main.Overview.Clients;
using Prism.Modularity;
using Prism.Regions;

namespace Maze.Administration
{
    public class ViewModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly ClientsContextMenu _clientsContextMenu;
        private readonly IClientManager _clientManager;
        private readonly IWindowService _windowService;
        private readonly IRestClient _restClient;

        public ViewModule(IRegionManager regionManager, ClientsContextMenu clientsContextMenu, IClientManager clientManager, IWindowService windowService, IRestClient restClient)
        {
            _regionManager = regionManager;
            _clientsContextMenu = clientsContextMenu;
            _clientManager = clientManager;
            _windowService = windowService;
            _restClient = restClient;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("MainContent", typeof(OverviewView));

            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(ClientsView));
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(GroupsView));
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(ModulesView));
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(AdministratorsView));

            _regionManager.RegisterViewWithRegion(RegionNames.ClientListTabs, typeof(DefaultClientListView));

            InitializeGroupsContextMenu();
        }

        private void InitializeGroupsContextMenu()
        {
            var menuItem = new MenuItem
            {
                Style = (Style) Application.Current.Resources["ClientGroupsMenuItem"],
                Tag = new GroupMenuItemViewModel(_windowService, _restClient)
            };
            menuItem.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("GroupViewModels") {Source = _clientManager});
            _clientsContextMenu.Add(new MenuItemEntry<ItemCommand<ClientViewModel>>(menuItem));
        }
    }
}