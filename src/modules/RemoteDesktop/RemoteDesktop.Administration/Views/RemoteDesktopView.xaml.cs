using Orcus.Administration.Library.Views;

namespace RemoteDesktop.Administration.Views
{
    /// <summary>
    ///     Interaction logic for RemoteDesktopView.xaml
    /// </summary>
    public partial class RemoteDesktopView
    {
        public RemoteDesktopView(IShellWindow viewManager) : base(viewManager)
        {
            InitializeComponent();
        }
    }
}