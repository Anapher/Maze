using System;
using System.Windows;
using System.Windows.Media;
using Anapher.Wpf.Swan.ViewInterface;
using Autofac;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Menus;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Views;
using Orcus.Administration.Prism;
using Prism.Commands;
using Prism.Regions;
using Unclassified.TxLib;

namespace Orcus.Administration.Services
{
    public class ClientCommandRegistrar : IClientCommandRegistrar
    {
        private readonly ClientsContextMenu _clientsContextMenu;
        private readonly IComponentContext _componentContext;
        private readonly IShellWindowFactory _shellWindowFactory;
        private readonly IOrcusRestClient _orcusRestClient;
        private readonly IViewModelResolver _viewModelResolver;

        public ClientCommandRegistrar(ClientsContextMenu clientsContextMenu, IViewModelResolver viewModelResolver,
            IComponentContext componentContext, IShellWindowFactory shellWindowFactory,
            IOrcusRestClient orcusRestClient)
        {
            _clientsContextMenu = clientsContextMenu;
            _viewModelResolver = viewModelResolver;
            _componentContext = componentContext;
            _shellWindowFactory = shellWindowFactory;
            _orcusRestClient = orcusRestClient;
        }

        private NavigationalEntry<ItemCommand<ClientViewModel>> GetNavigationEntry(CommandCategory category)
        {
            switch (category)
            {
                case CommandCategory.Interaction:
                    return _clientsContextMenu.InteractionCommands;
                case CommandCategory.System:
                    return _clientsContextMenu.SystemCommands;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }

        public void RegisterView(Type viewType, string txLibResource, object icon, CommandCategory category)
        {
            GetNavigationEntry(category).Add(new ItemCommand<ClientViewModel>
            {
                Header = Tx.T(txLibResource),
                Icon = icon,
                Command = new DelegateCommand<ClientViewModel>(model =>
                {
                    var viewModelType = _viewModelResolver.ResolveViewModelType(viewType);
                    StatusBarManager statusBar = null;

                    var window = _shellWindowFactory.Create();
                    window.InitializeTitleBar(Tx.T(txLibResource), icon as ImageSource);

                    var lifescope = _componentContext.Resolve<ILifetimeScope>().BeginLifetimeScope(builder =>
                    {
                        builder.RegisterInstance(window.ViewManager).As<IWindow>().As<IMetroWindow>()
                            .As<IWindowViewManager>();
                        builder.Register(context => statusBar = new StatusBarManager()).As<IShellStatusBar>()
                            .SingleInstance();
                        builder.RegisterType(viewType);
                        builder.RegisterType(viewModelType);
                        builder.RegisterInstance(model);
                        builder.Register(context => _orcusRestClient.CreateTargeted(model.ClientId)).SingleInstance();
                    });

                    var viewModel = lifescope.Resolve(viewModelType);
                    var view = (FrameworkElement) lifescope.Resolve(viewType);
                    view.DataContext = viewModel;

                    if (viewModel is INavigationAware navigationAware)
                        window.Window.Loaded += (sender, args) => navigationAware.OnNavigatedTo(null);

                    window.InitalizeContent(view, statusBar);
                    window.Window.Closed += (sender, args) => lifescope.Dispose();
                    window.Show();
                })
            });
        }
    }
}