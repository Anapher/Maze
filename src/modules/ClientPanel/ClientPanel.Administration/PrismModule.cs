using Anapher.Wpf.Toolkit.Extensions;
using Anapher.Wpf.Toolkit.Windows;
using ClientPanel.Administration.Resources;
using ClientPanel.Administration.ViewModels;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Prism.Commands;
using Prism.Ioc;
using Prism.Modularity;
using Unclassified.TxLib;
using Unity;
using Unity.Injection;

namespace ClientPanel.Administration
{
    public class PrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Tx.LoadFromEmbeddedResource("ClientPanel.Administration.Resources.ClientPanel.Translation.txd");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var clientsContextMenu = containerProvider.Resolve<ClientsContextMenu>();
            var windowService = containerProvider.Resolve<IWindowService>();
            var icons = containerProvider.Resolve<VisualStudioIcons>();

            clientsContextMenu.AddAtBeginning(new ItemCommand<ClientViewModel>
            {
                Header = Tx.T("ClientPanel:ClientPanel"),
                Icon = icons.CodeDefinitionWindow,
                Command = new DelegateCommand<ClientViewModel>(clientViewModel =>
                {
                    windowService.Show<ClientPanelViewModel>(builder =>
                    {
                        builder.RegisterInstance(clientViewModel);
                        builder.RegisterSingleton<ITargetedRestClient>(new InjectionFactory(container =>
                            container.Resolve<IMazeRestClient>().CreateTargeted(clientViewModel.ClientId)));
                    }, window =>
                    {
                        window.TitleBarIcon = icons.CodeDefinitionWindow;
                        window.Title = Tx.T("ClientPanel:ClientPanel");
                        window.ShowInTaskbar = true;
                    }, null, out _);
                })
            });
        }
    }
}