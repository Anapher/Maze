using System.Windows;
using System.Windows.Controls;
using Tasks.Infrastructure.Administration.ViewModels;

namespace Tasks.Infrastructure.Administration.Views.TaskOverview
{
    /// <summary>
    ///     Interaction logic for OverviewTreeView.xaml
    /// </summary>
    public partial class OverviewTreeView : UserControl
    {
        public OverviewTreeView()
        {
            InitializeComponent();
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((TaskOverviewViewModel) DataContext).SelectedItem = e.NewValue;
        }
    }
}