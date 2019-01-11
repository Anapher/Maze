using System.Windows;

namespace Maze.Administration.Views.Main.Overview.Administrators
{
    /// <summary>
    ///     Interaction logic for ChangeAdministratorPasswordView.xaml
    /// </summary>
    public partial class ChangeAdministratorPasswordView
    {
        public ChangeAdministratorPasswordView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PasswordBox.Focus();
        }
    }
}