using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Resources;
using Unclassified.TxLib;

namespace Maze.Administration.Library.Menus
{
    public class ClientsContextMenu : MenuSection<ItemCommand<ClientViewModel>>
    {
        public ClientsContextMenu(ILibraryIcons icons)
        {
            Add(SystemCommands = new NavigationalEntry<ItemCommand<ClientViewModel>>(isOrdered: true)
            {
                Header = Tx.T("Menu.System"),
                Icon = icons.ComputerSystem
            });
            Add(InteractionCommands = new NavigationalEntry<ItemCommand<ClientViewModel>>(isOrdered: true)
            {
                Header = Tx.T("Menu.UserInteraction"),
                Icon = icons.UserVoice
            });
            Add(SurveillanceCommands = new NavigationalEntry<ItemCommand<ClientViewModel>>(isOrdered: true)
            {
                Header = Tx.T("Menu.Surveillance"),
                Icon = icons.HideTimeline
            });
            Add(ClientCommands = new MenuSection<ItemCommand<ClientViewModel>>());
        }

        public NavigationalEntry<ItemCommand<ClientViewModel>> SystemCommands { get; }
        public NavigationalEntry<ItemCommand<ClientViewModel>> InteractionCommands { get; }
        public NavigationalEntry<ItemCommand<ClientViewModel>> SurveillanceCommands { get; }
        public MenuSection<ItemCommand<ClientViewModel>> ClientCommands { get; }
    }
}
