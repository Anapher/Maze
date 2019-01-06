using System;
using System.Windows;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace FileExplorer.Administration.Views
{
    /// <summary>
    ///     Interaction logic for InputTextView.xaml
    /// </summary>
    public partial class InputTextView
    {
        public InputTextView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                NameTextBox.Focus();
                NameTextBox.SelectAll();
            }));
        }
    }
}