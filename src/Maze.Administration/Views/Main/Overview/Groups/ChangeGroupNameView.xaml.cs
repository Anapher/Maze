using System.Windows;
using Maze.Administration.Library.Views;

namespace Maze.Administration.Views.Main.Overview.Groups
{
    /// <summary>
    ///     Interaction logic for ChangeGroupNameView.xaml
    /// </summary>
    public partial class ChangeGroupNameView
    {
        public ChangeGroupNameView(IShellWindow shellWindow) : base(shellWindow)
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