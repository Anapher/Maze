using Maze.Administration.Library.Views;

namespace ClipboardManager.Administration.Views
{
    /// <summary>
    ///     Interaction logic for ClipboardManagerView.xaml
    /// </summary>
    public partial class ClipboardManagerView
    {
        public ClipboardManagerView(IShellWindow viewManager) : base(viewManager)
        {
            InitializeComponent();
        }
    }
}