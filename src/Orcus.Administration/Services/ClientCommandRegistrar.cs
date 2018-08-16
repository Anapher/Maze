using System;
using System.Windows;
using System.Windows.Media;
using Autofac;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Prism;
using Prism.Commands;
using Unclassified.TxLib;

namespace Orcus.Administration.Services
{
    public class ClientCommandRegistrar : IClientCommandRegistrar
    {
        private readonly ClientsContextMenu _clientsContextMenu;
        private readonly IComponentContext _componentContext;
        private readonly IShellWindowOpener _shellWindowOpener;
        private readonly IViewModelResolver _viewModelResolver;

        public ClientCommandRegistrar(ClientsContextMenu clientsContextMenu, IViewModelResolver viewModelResolver,
            IComponentContext componentContext, IShellWindowOpener shellWindowOpener)
        {
            _clientsContextMenu = clientsContextMenu;
            _viewModelResolver = viewModelResolver;
            _componentContext = componentContext;
            _shellWindowOpener = shellWindowOpener;
        }

        public void RegisterView(Type viewType, string txLibResource, object icon, CommandCategory category)
        {
            _clientsContextMenu.InteractionCommands.Add(new CommandMenuEntry<ClientViewModel>
            {
                Header = Tx.T(txLibResource),
                Icon = icon,
                Command = new DelegateCommand<ClientViewModel>(model =>
                {
                    var viewModelType = _viewModelResolver.ResolveViewModelType(viewType);

                    var lifescope = _componentContext.Resolve<ILifetimeScope>().BeginLifetimeScope(builder =>
                    {
                        builder.RegisterType(viewType);
                        builder.RegisterType(viewModelType);
                        builder.RegisterInstance(model);
                    });

                    var viewModel = lifescope.Resolve(viewModelType);
                    var view = (FrameworkElement) lifescope.Resolve(viewType);
                    view.DataContext = viewModel;

                    var window = _shellWindowOpener.Show(view, Tx.T(txLibResource), icon as ImageSource);
                    window.Closed += (sender, args) => lifescope.Dispose();
                })
            });
        }
    }
}