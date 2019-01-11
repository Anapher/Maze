using System.Windows;

namespace Maze.Administration.Views.Main.Overview.Groups
{
    /// <summary>
    ///     Interaction logic for ChangeGroupNameView.xaml
    /// </summary>
    public partial class ChangeGroupNameView
    {
        public ChangeGroupNameView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
            NameTextBox.SelectAll();
        }
    }
}