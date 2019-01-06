using System.Windows;
using RegistryEditor.Administration.Resources;
using RegistryEditor.Administration.ViewModels;

namespace RegistryEditor.Administration.Views
{
    /// <summary>
    ///     Interaction logic for CreateEditValueView.xaml
    /// </summary>
    public partial class CreateEditValueView
    {
        public CreateEditValueView(VisualStudioIcons icons)
        {
            InitializeComponent();
            Icon = icons.Edit;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateEditValueViewModel) DataContext;
            if (viewModel.IsCreate)
                KeyNameTextBox.Focus();
            else ValueContentControl.Focus();
        }
    }
}