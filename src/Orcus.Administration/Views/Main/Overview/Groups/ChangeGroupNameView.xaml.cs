using System.Windows;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Views.Main.Overview.Groups
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