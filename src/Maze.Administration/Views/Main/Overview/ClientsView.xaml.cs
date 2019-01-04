using System.Windows;
using System.Windows.Controls;
using Maze.Administration.Library.ViewModels;

namespace Maze.Administration.Views.Main.Overview
{
    /// <summary>
    ///     Interaction logic for ClientsView.xaml
    /// </summary>
    public partial class ClientsView : UserControl
    {
        public ClientsView()
        {
            InitializeComponent();
        }

        private void SearchTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            OnSearchTextChanged();
        }

        private void ClientListTabControlOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSearchTextChanged();
        }

        private void OnSearchTextChanged()
        {
            if (ClientListTabControl.SelectedItem is FrameworkElement item)
                if (item.DataContext is ClientListBase clientListBase)
                    clientListBase.SearchText = SearchTextBox.Text;
        }
    }
}