using System.Windows;
using Orcus.Administration.Library.StatusBar;

namespace Orcus.Administration.Controls
{
    /// <summary>
    ///     Interaction logic for StatusBar.xaml
    /// </summary>
    public partial class StatusBar
    {
        public static readonly DependencyProperty ShellStatusBarProperty = DependencyProperty.Register("ShellStatusBar",
            typeof(StatusBarManager), typeof(StatusBar), new PropertyMetadata(default(StatusBarManager)));

        public StatusBar()
        {
            InitializeComponent();
        }

        public StatusBarManager ShellStatusBar
        {
            get => (StatusBarManager) GetValue(ShellStatusBarProperty);
            set => SetValue(ShellStatusBarProperty, value);
        }
    }
}