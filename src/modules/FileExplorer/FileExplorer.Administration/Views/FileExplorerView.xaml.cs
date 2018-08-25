using FileExplorer.Administration.Resources;
using Orcus.Administration.Library.Views;

namespace FileExplorer.Administration.Views
{
    /// <summary>
    ///     Interaction logic for FileExplorerView.xaml
    /// </summary>
    public partial class FileExplorerView
    {
        public FileExplorerView(IWindowViewManager viewManager) : base(viewManager)
        {
            InitializeComponent();
            
            Icon = VisualStudioImages.ListFolder();
        }
    }
}