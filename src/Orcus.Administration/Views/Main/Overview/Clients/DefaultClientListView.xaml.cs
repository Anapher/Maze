using System.Windows.Controls;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menu.MenuBase;

namespace Orcus.Administration.Views.Main.Overview.Clients
{
    /// <summary>
    ///     Interaction logic for DefaultClientListView.xaml
    /// </summary>
    public partial class DefaultClientListView : UserControl
    {
        public DefaultClientListView(ClientsContextMenu clientsContextMenu, IMenuFactory menuFactory)
        {
            InitializeComponent();

            var contextMenu = (ContextMenu) ClientsDataGrid.Resources["OnlineItemContextMenu"];
            var items = menuFactory.Create(clientsContextMenu);
            foreach (var item in items)
                contextMenu.Items.Add(item);
        }
    }
}