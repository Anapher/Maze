using UserInteraction.Administration.Resources;

namespace UserInteraction.Administration.Views
{
    /// <summary>
    ///     Interaction logic for MessageBoxView.xaml
    /// </summary>
    public partial class MessageBoxView
    {
        public MessageBoxView(VisualStudioIcons icons)
        {
            InitializeComponent();
            Icon = icons.MessageBox;
        }
    }
}