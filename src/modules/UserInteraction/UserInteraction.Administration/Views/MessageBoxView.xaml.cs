using Orcus.Administration.Library.Views;
using UserInteraction.Administration.Resources;

namespace UserInteraction.Administration.Views
{
    /// <summary>
    ///     Interaction logic for MessageBoxView.xaml
    /// </summary>
    public partial class MessageBoxView
    {
        public MessageBoxView(IShellWindow viewManager, VisualStudioIcons icons) : base(viewManager)
        {
            InitializeComponent();
            Icon = icons.MessageBox;
        }
    }
}