using Maze.Administration.Library.Views;

namespace Tasks.Infrastructure.Administration.Views
{
    /// <summary>
    ///     Interaction logic for ExecuteCommandView.xaml
    /// </summary>
    public partial class ExecuteCommandView
    {
        public ExecuteCommandView(IShellWindow viewManager) : base(viewManager)
        {
            InitializeComponent();
        }
    }
}