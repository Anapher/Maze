using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Orcus.Administration.Library;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.Views;
using Orcus.Administration.ViewModels.Overview.Groups;
using Orcus.Administration.Views.Main;
using Orcus.Administration.Views.Main.Overview;
using Orcus.Administration.Views.Main.Overview.Clients;
using Prism.Modularity;
using Prism.Regions;

namespace Orcus.Administration
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