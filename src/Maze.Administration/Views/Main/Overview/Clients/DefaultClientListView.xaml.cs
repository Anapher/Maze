using System.Windows.Controls;
using Maze.Administration.Library.Menu;
using Maze.Administration.Library.Menu.MenuBase;
using Maze.Administration.Library.Menus;
using Maze.Administration.Library.Models;

namespace Maze.Administration.Views.Main.Overview.Clients
{
    /// <summary>
    ///     Interaction logic for DefaultClientListView.xaml
    /// </summary>
    public partial class DefaultClientListView : UserControl
    {
        public DefaultClientListView(ClientsContextMenu clientsContextMenu, OfflineClientsContextMenu offlineClientsContextMenu, IItemMenuFactory menuFactory)
        {
            InitializeComponent();

            var contextMenu = (ContextMenu)ClientsDataGrid.Resources["OnlineItemContextMenu"];
            InitializeContextMenu(clientsContextMenu, contextMenu, menuFactory);

            contextMenu = (ContextMenu)ClientsDataGrid.Resources["OfflineItemContextMenu"];
            InitializeContextMenu(offlineClientsContextMenu, contextMenu, menuFactory);
        }

        private void InitializeContextMenu(MenuSection<ItemCommand<ClientViewModel>> contextMenuSource, ContextMenu contextMenu, IItemMenuFactory menuFactory)
        {
            var items = menuFactory.Create(contextMenuSource, null);
            foreach (var item in items)
                contextMenu.Items.Add(item);

            ContextMenuExtensions.SetSelectedItems(contextMenu, ClientsDataGrid.SelectedItems);
        }
    }
}