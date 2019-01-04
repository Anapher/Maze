using System.Windows;
using Maze.Administration.Library.Views;

namespace Maze.Administration.Views.Main.Overview.Administrators
{
    /// <summary>
    ///     Interaction logic for ChangeAdministratorPasswordView.xaml
    /// </summary>
    public partial class ChangeAdministratorPasswordView
    {
        public ChangeAdministratorPasswordView(IShellWindow shellWindow) : base(shellWindow)
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