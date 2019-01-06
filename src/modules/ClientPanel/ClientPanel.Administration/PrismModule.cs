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

namespace ClientPanel.Administration
{
    public class PrismModule : IModule
    {
        private readonly ClientsContextMenu _clientsContextMenu;
        private readonly IWindowService _windowService;
        private readonly IMazeRestClient _restClient;
        private readonly VisualStudioIcons _icons;

        public PrismModule(ClientsContextMenu clientsContextMenu, IWindowService windowService, IMazeRestClient restClient, VisualStudioIcons icons)
        {
            _clientsContextMenu = clientsContextMenu;
            _windowService = windowService;
            _restClient = restClient;
            _icons = icons;
        }

        public void Initialize()
        {
            Tx.LoadFromEmbeddedResource("ClientPanel.Administration.Resources.ClientPanel.Translation.txd");

            _clientsContextMenu.AddAtBeginning(new ItemCommand<ClientViewModel>
            {
                Header = Tx.T("ClientPanel:ClientPanel"),
                Icon = _icons.CodeDefinitionWindow,
                Command = new DelegateCommand<ClientViewModel>(clientViewModel =>
                {
                    _windowService.Show<ClientPanelViewModel>(builder =>
                    {
                        builder.RegisterInstance(clientViewModel);
                        builder.Register(context => _restClient.CreateTargeted(clientViewModel.ClientId)).SingleInstance();
                    }, window =>
                    {
                        window.TitleBarIcon = _icons.CodeDefinitionWindow;
                        window.Title = Tx.T("ClientPanel:ClientPanel");
                    }, null, out _);
                })
            });
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new System.NotImplementedException();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new System.NotImplementedException();
        }
    }
}