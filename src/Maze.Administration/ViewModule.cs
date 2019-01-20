using System.Collections.Generic;
using System.Linq;
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
using Maze.Administration.ViewModels;
using Maze.Administration.ViewModels.Overview.Groups;
using Maze.Administration.Views.Main;
using Maze.Administration.Views.Main.Overview;
using Maze.Administration.Views.Main.Overview.Clients;
using Microsoft.Extensions.Caching.Memory;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Maze.Administration
{
    public class ViewModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppDispatcher, AppDispatcher>();
            containerRegistry.RegisterSingleton<IMenuFactory, DefaultMenuFactory>();
            containerRegistry.RegisterSingleton<IItemMenuFactory, ItemMenuFactory>();
            containerRegistry.RegisterSingleton<ClientsContextMenu>();
            containerRegistry.RegisterSingleton<OfflineClientsContextMenu>();
            containerRegistry.RegisterSingleton<IClientCommandRegistrar, ClientCommandRegistrar>();
            containerRegistry.RegisterSingleton<IShellWindowFactory, ShellWindowFactory>();
            containerRegistry.RegisterInstance<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
            containerRegistry.RegisterSingleton<ILibraryIcons, VisualStudioIcons>();
            containerRegistry.GetContainer().AsImplementedInterfaces<UnityServiceProvider, TransientLifetimeManager>();

            //enable IEnumerable resolving
            containerRegistry.GetContainer().RegisterType(typeof(IEnumerable<>),
                new InjectionFactory((container, type, name) =>
                    container.ResolveAll(type.GetGenericArguments().Single())));

            containerRegistry.Register<object, LoginView>(PrismModule.MainContentLoginView);
            containerRegistry.RegisterForNavigation<LoginView>(PrismModule.MainContent);

            containerRegistry.Register<object, OverviewView>(PrismModule.MainContentOverviewView);
            containerRegistry.RegisterForNavigation<OverviewView>(PrismModule.MainContent);
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(ClientsView));
            regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(GroupsView));
            regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(ModulesView));
            regionManager.RegisterViewWithRegion(RegionNames.OverviewTabs, typeof(AdministratorsView));

            regionManager.RegisterViewWithRegion(RegionNames.ClientListTabs, typeof(DefaultClientListView));

            regionManager.RegisterViewWithRegion(PrismModule.MainContent, typeof(LoginView));

            var windowService = containerProvider.Resolve<IWindowService>();
            var restClient = containerProvider.Resolve<IRestClient>();
            var clientManager = containerProvider.Resolve<IClientManager>();

            var clientsContextMenu = containerProvider.Resolve<ClientsContextMenu>();
            InitializeGroupsContextMenu(clientsContextMenu, windowService, restClient, clientManager);

            var offlineClientsContextMenu = containerProvider.Resolve<OfflineClientsContextMenu>();
            InitializeGroupsContextMenu(offlineClientsContextMenu, windowService, restClient, clientManager);
        }

        private static void InitializeGroupsContextMenu(MenuSection<ItemCommand<ClientViewModel>> contextMenu, IWindowService windowService,
            IRestClient restClient, IClientManager clientManager)
        {
            var menuItem = new MenuItem
            {
                Style = (Style) Application.Current.Resources["ClientGroupsMenuItem"], Tag = new GroupMenuItemViewModel(windowService, restClient)
            };
            menuItem.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("GroupViewModels") {Source = clientManager});

            var section = new MenuSection<ItemCommand<ClientViewModel>> {new MenuItemEntry<ItemCommand<ClientViewModel>>(menuItem)};
            contextMenu.Add(section);
        }
    }
}