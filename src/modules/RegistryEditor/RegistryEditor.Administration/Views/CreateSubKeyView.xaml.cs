using System.Windows;
using RegistryEditor.Administration.Resources;

namespace RegistryEditor.Administration.Views
{
    /// <summary>
    ///     Interaction logic for CreateSubKeyView.xaml
    /// </summary>
    public partial class CreateSubKeyView
    {
        public CreateSubKeyView(VisualStudioIcons icons)
        {
            InitializeComponent();
            Icon = icons.NewSolutionFolder;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
        }
    }
}