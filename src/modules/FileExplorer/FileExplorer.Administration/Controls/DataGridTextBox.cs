using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.Administration.Controls
{
    public class DataGridTextBox : TextBox
    {
        public static readonly DependencyProperty IsDirectorySelectedProperty =
            DependencyProperty.Register("IsDirectorySelected", typeof(bool), typeof(DataGridTextBox),
                new PropertyMetadata(default(bool)));

        public bool IsDirectorySelected
        {
            get => (bool) GetValue(IsDirectorySelectedProperty);
            set => SetValue(IsDirectorySelectedProperty, value);
        }

        public DataGridTextBox()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var extensionIndex = Text.LastIndexOf('.');
            if (!IsDirectorySelected && extensionIndex > -1)
                Select(0, extensionIndex);
            else SelectAll();

            Focus();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            e.Handled = true;
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDoubleClick(e);
            e.Handled = true;
        }
    }
}