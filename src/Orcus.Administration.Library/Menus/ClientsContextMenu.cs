using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Resources;
using Unclassified.TxLib;

namespace Orcus.Administration.Library.Menus
{
    public class ClientsContextMenu : MenuSection<ItemCommand<ClientViewModel>>
    {
        public ClientsContextMenu(ILibraryIcons icons)
        {
            Add(SystemCommands = new NavigationalEntry<ItemCommand<ClientViewModel>>
            {
                Header = Tx.T("Menu.System"),
                Icon = icons.ComputerService
            });
            Add(InteractionCommands = new NavigationalEntry<ItemCommand<ClientViewModel>>
            {
                Header = Tx.T("Menu.UserInteraction"),
                Icon = icons.UserVoice
            });
            Add(ClientCommands = new MenuSection<ItemCommand<ClientViewModel>>());
        }

        public NavigationalEntry<ItemCommand<ClientViewModel>> SystemCommands { get; }
        public NavigationalEntry<ItemCommand<ClientViewModel>> InteractionCommands { get; }
        public MenuSection<ItemCommand<ClientViewModel>> ClientCommands { get; }
    }
}
