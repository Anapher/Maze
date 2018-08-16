using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Models;

namespace Orcus.Administration.Library.Menu
{
    public class ClientsContextMenu : MenuSection<ClientViewModel>
    {
        public ClientsContextMenu()
        {
            Add(SystemCommands = new NavigationalEntry<ClientViewModel>());
            Add(InteractionCommands = new NavigationalEntry<ClientViewModel>());
            Add(ClientCommands = new MenuSection<ClientViewModel>());
        }

        public NavigationalEntry<ClientViewModel> SystemCommands { get; }
        public NavigationalEntry<ClientViewModel> InteractionCommands { get; }
        public MenuSection<ClientViewModel> ClientCommands { get; }
    }
}
