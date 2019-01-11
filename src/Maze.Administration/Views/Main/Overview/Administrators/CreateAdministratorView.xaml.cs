using System.Windows;

namespace Maze.Administration.Views.Main.Overview.Administrators
{
    /// <summary>
    ///     Interaction logic for CreateAdministratorView.xaml
    /// </summary>
    public partial class CreateAdministratorView
    {
        public CreateAdministratorView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Focus();
        }
    }
}