using System.Windows;
using System.Windows.Controls;
using Anapher.Wpf.Swan;
using Anapher.Wpf.Swan.ViewInterface;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Views
{
    /// <summary>
    ///     Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : MetroWindow, IMetroWindow
    {
        public ShellWindow(object content, IShellStatusBar statusBar)
        {
            InitializeComponent();

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});


        }

        public ShellWindow(object content)
        {
            InitializeComponent();
            Content = content;
        }

        public MessageBoxResult ShowMessageBox(string text, string caption, MessageBoxButton buttons,
            MessageBoxImage icon, MessageBoxResult defResult, MessageBoxOptions options)
        {
            return MessageBoxEx.Show(text, caption, buttons, icon, defResult, options);
        }

        public void AddFlyout(Flyout flyout)
        {
            if (Flyouts == null)
                Flyouts = new FlyoutsControl();

            Flyouts.Items.Add(flyout);
        }

        public bool? ShowDialog(VistaFileDialog fileDialog)
        {
            return fileDialog.ShowDialog(this);
        }

        public bool? ShowDialog(FileDialog fileDialog)
        {
            return fileDialog.ShowDialog(this);
        }
    }
}