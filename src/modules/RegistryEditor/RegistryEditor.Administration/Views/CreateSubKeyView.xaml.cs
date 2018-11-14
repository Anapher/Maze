using Orcus.Administration.Library.Views;
using RegistryEditor.Administration.Resources;

namespace RegistryEditor.Administration.Views
{
    /// <summary>
    ///     Interaction logic for CreateSubKeyView.xaml
    /// </summary>
    public partial class CreateSubKeyView
    {
        public CreateSubKeyView(IShellWindow viewManager, VisualStudioIcons icons) : base(viewManager)
        {
            InitializeComponent();
            Icon = icons.NewSolutionFolder;
        }
    }
}