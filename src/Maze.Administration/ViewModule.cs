using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Anapher.Wpf.Toolkit.Metro.Services;
using Anapher.Wpf.Toolkit.Prism;
using Anapher.Wpf.Toolkit.Windows;
using Maze.Administration.Factories;
using Maze.Administration.Library;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Menu;
using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Resources;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.Unity;
using Maze.Administration.Services;
using Maze.Administration.ViewModels.Overview.Groups;
using Maze.Administration.Views.Main;
using Maze.Administration.Views.Main.Overview;
using Maze.Administration.Views.Main.Overview.Clients;
using Microsoft.Extensions.Caching.Memory;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Unity.Lifetime;

namespace Maze.Administration
{
    public class ViewModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IClientManager _clientManager;
        private readonly IWindowService _windowService;
        private readonly IRestClient _restClient;

        public ViewModule(IRegionManager regionManager, IClientManager clientManager, IWindowService windowService, IRestClient restClient)
        {
            _regionManager = regionManager;
            _clientManager = clientManager;
            _windowService = windowService;
            _restClient = restClient;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppDispatcher, AppDispatcher>();
            containerRegistry.RegisterSingleton<IMenuFactory, DefaultMenuFactory>();
            containerRegistry.RegisterSingleton<ClientsContextMenu>();
            containerRegistry.RegisterSingleton<OfflineClientsContextMenu>();
            containerRegistry.RegisterSingleton<IClientCommandRegistrar, ClientCommandRegistrar>();
            containerRegistry.RegisterSingleton<IShellWindowFactory, ShellWindowFactory>();
            containerRegistry.RegisterInstance<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
            containerRegistry.RegisterSingleton<ILibraryIcons, VisualStudioIcons>();
            containerRegistry.GetContainer().AsImplementedInterfaces<UnityServiceProvider>(new TransientLifetimeManager());
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("MainContent", typeof(LoginView)); //OverviewView

            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(ClientsView));
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(GroupsView));
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(ModulesView));
            _regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(AdministratorsView));

            _regionManager.RegisterViewWithRegion(RegionNames.ClientListTabs, typeof(DefaultClientListView));

            var clientsContextMenu = containerProvider.Resolve<ClientsContextMenu>();
            InitializeGroupsContextMenu(clientsContextMenu);

            var offlineClientsContextMenu = containerProvider.Resolve<OfflineClientsContextMenu>();
            InitializeGroupsContextMenu(offlineClientsContextMenu);
        }

        private void InitializeGroupsContextMenu(MenuSection<ItemCommand<ClientViewModel>> contextMenu)
        {
            var menuItem = new MenuItem
            {
                Style = (Style) Application.Current.Resources["ClientGroupsMenuItem"],
                Tag = new GroupMenuItemViewModel(_windowService, _restClient)
            };
            menuItem.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("GroupViewModels") {Source = _clientManager});

            var section = new MenuSection<ItemCommand<ClientViewModel>> {new MenuItemEntry<ItemCommand<ClientViewModel>>(menuItem)};
            contextMenu.Add(section);
        }
    }
}