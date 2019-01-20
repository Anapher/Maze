using System;
using Anapher.Wpf.Toolkit.Windows;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Services;
using Prism.Commands;
using Unclassified.TxLib;
using Unity;
using Unity.Injection;

namespace Maze.Administration.Services
{
    public class ClientCommandRegistrar : IClientCommandRegistrar
    {
        private readonly ClientsContextMenu _clientsContextMenu;
        private readonly IWindowService _windowService;

        public ClientCommandRegistrar(ClientsContextMenu clientsContextMenu, IWindowService windowService)
        {
            _clientsContextMenu = clientsContextMenu;
            _windowService = windowService;
        }

        public void Register<TViewModel>(string txLibResource, IIconFactory iconFactory, CommandCategory category)
        {
            GetNavigationEntry(category).Add(new ItemCommand<ClientViewModel>
            {
                Header = Tx.T(txLibResource),
                Icon = iconFactory.Create(),
                Command = new DelegateCommand<ClientViewModel>(model =>
                {
                    _windowService.Show(typeof(TViewModel), builder =>
                    {
                        builder.RegisterInstance(model);
                        builder.RegisterSingleton<ITargetedRestClient>(new InjectionFactory(container =>
                            container.Resolve<IMazeRestClient>().CreateTargeted(model.ClientId)));
                    }, window =>
                    {
                        window.TitleBarIcon = iconFactory.Create();
                        window.Title = Tx.T(txLibResource);
                        window.ShowInTaskbar = true;
                    }, null, out _);
                })
            });
        }

        private NavigationalEntry<ItemCommand<ClientViewModel>> GetNavigationEntry(CommandCategory category)
        {
            switch (category)
            {
                case CommandCategory.Interaction:
                    return _clientsContextMenu.InteractionCommands;
                case CommandCategory.System:
                    return _clientsContextMenu.SystemCommands;
                case CommandCategory.Surveillance:
                    return _clientsContextMenu.SurveillanceCommands;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}