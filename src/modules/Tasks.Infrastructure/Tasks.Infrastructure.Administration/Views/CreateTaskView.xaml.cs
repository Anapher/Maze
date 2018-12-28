using Maze.Administration.Library.Views;

namespace Tasks.Infrastructure.Administration.Views
{
    /// <summary>
    ///     Interaction logic for CreateTaskView.xaml
    /// </summary>
    public partial class CreateTaskView
    {
        public CreateTaskView(IShellWindow viewManager) : base(viewManager)
        {
            InitializeComponent();
        }
    }
}