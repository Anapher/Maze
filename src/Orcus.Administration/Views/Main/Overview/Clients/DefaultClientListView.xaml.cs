using System.Windows;
using System.Windows.Controls;
using Orcus.Administration.Library.Menu;
using Orcus.Administration.Library.Menu.MenuBase;
using Orcus.Administration.Library.Models;
using Prism.Commands;

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

            clientsContextMenu.ClientCommands.Add(new CommandMenuEntry<ClientViewModel>{Header = "Hello World", Command = new DelegateCommand<ClientViewModel>(ExecuteMethod)});

            var contextMenu = (ContextMenu) ClientsDataGrid.Resources["ItemContextMenu"];
            var items = menuFactory.Create(clientsContextMenu);
            foreach (var item in items)
                contextMenu.Items.Add(item);
        }

        private void ExecuteMethod(ClientViewModel obj)
        {
            MessageBox.Show("Hello");
        }
    }
}