using Orcus.Administration.Library.Views;

namespace TaskManager.Administration.Views
{
    /// <summary>
    ///     Interaction logic for TaskManagerView.xaml
    /// </summary>
    public partial class TaskManagerView
    {
        public TaskManagerView(IWindowViewManager windowViewManager) : base(windowViewManager)
        {
            InitializeComponent();
        }
    }
}