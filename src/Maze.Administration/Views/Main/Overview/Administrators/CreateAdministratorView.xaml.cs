using System.Windows;
using Maze.Administration.Library.Views;

namespace Maze.Administration.Views.Main.Overview.Administrators
{
    /// <summary>
    ///     Interaction logic for CreateAdministratorView.xaml
    /// </summary>
    public partial class CreateAdministratorView
    {
        public CreateAdministratorView(IShellWindow window) : base(window)
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