using System.Windows;
using Orcus.Administration.Library.Views;
using RegistryEditor.Administration.Resources;
using RegistryEditor.Administration.ViewModels;
using Unclassified.TxLib;

namespace RegistryEditor.Administration.Views
{
    /// <summary>
    ///     Interaction logic for CreateEditValueView.xaml
    /// </summary>
    public partial class CreateEditValueView
    {
        public CreateEditValueView(IWindowViewManager viewManager, VisualStudioIcons icons) : base(viewManager)
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