using Maze.Administration.Library.Views;

namespace TaskManager.Administration.Views
{
    /// <summary>
    ///     Interaction logic for TaskManagerView.xaml
    /// </summary>
    public partial class TaskManagerView
    {
        public TaskManagerView(IShellWindow windowViewManager) : base(windowViewManager)
        {
            InitializeComponent();
        }
    }
}