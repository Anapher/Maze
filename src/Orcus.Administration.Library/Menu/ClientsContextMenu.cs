using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Models;
using Orcus.Administration.Library.Resources;
using Unclassified.TxLib;

namespace Orcus.Administration.Library.Menu
{
    public class ClientsContextMenu : MenuSection<ClientViewModel>
    {
        public ClientsContextMenu()
        {
            Add(SystemCommands = new NavigationalEntry<ClientViewModel>
            {
                Header = Tx.T("Menu.System"),
                Icon = VisualStudioIcons.ComputerService()
            });
            Add(InteractionCommands = new NavigationalEntry<ClientViewModel>
            {
                Header = Tx.T("Menu.UserInteraction"),
                Icon = VisualStudioIcons.UserVoice()
            });
            Add(ClientCommands = new MenuSection<ClientViewModel>());
        }

        public NavigationalEntry<ClientViewModel> SystemCommands { get; }
        public NavigationalEntry<ClientViewModel> InteractionCommands { get; }
        public MenuSection<ClientViewModel> ClientCommands { get; }
    }
}
