using Autofac;
using ClientPanel.Administration.Resources;
using ClientPanel.Administration.ViewModels;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Views;
using Prism.Commands;
using Prism.Modularity;
using Unclassified.TxLib;

namespace ClientPanel.Administration
{
    public class PrismModule : IModule
    {
        private readonly ClientsContextMenu _clientsContextMenu;
        private readonly IWindowService _windowService;
        private readonly IOrcusRestClient _restClient;
        private readonly VisualStudioIcons _icons;

        public PrismModule(ClientsContextMenu clientsContextMenu, IWindowService windowService, IOrcusRestClient restClient, VisualStudioIcons icons)
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
    }
}