using Orcus.Administration.Library.Views;
using UserInteraction.Administration.Resources;

namespace UserInteraction.Administration.Views
{
    /// <summary>
    ///     Interaction logic for MessageBoxView.xaml
    /// </summary>
    public partial class MessageBoxView
    {
        public MessageBoxView(IWindowViewManager viewManager) : base(viewManager)
        {
            InitializeComponent();
            Icon = VisualStudioImages.MessageBox();
        }
    }
}