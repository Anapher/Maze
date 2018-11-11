using Orcus.Administration.Library.Views;

namespace Tasks.Infrastructure.Administration.Views
{
    /// <summary>
    ///     Interaction logic for TaskOverviewView.xaml
    /// </summary>
    public partial class TaskOverviewView
    {
        public TaskOverviewView(IWindowViewManager viewManager) : base(viewManager)
        {
            InitializeComponent();
        }
    }
}