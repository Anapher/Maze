using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;

namespace Maze.Administration.Extensions
{
    public static class TextEditorExtensions
    {
        public static readonly DependencyProperty EnableNiceStyleProperty = DependencyProperty.RegisterAttached("EnableNiceStyle", typeof(bool),
            typeof(TextEditorExtensions), new PropertyMetadata(default(bool), OnEnableNiceStyleChanged));

        private static void OnEnableNiceStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) e.NewValue)
                throw new ArgumentException("Why would you not want nice styles?");

            var editor = (TextEditor) d;
            editor.Options.HighlightCurrentLine = true;
            editor.Options.EnableRectangularSelection = true;

            editor.TextArea.TextView.CurrentLineBackground = Brushes.Transparent;
            editor.TextArea.TextView.CurrentLineBorder = new Pen(new SolidColorBrush(Color.FromRgb(70, 70, 70)), 1);
            editor.TextArea.SelectionCornerRadius = 1;
            editor.TextArea.SelectionBrush = new SolidColorBrush(Color.FromRgb(38, 79, 120));
            editor.TextArea.SelectionBorder = new Pen(new SolidColorBrush(Color.FromRgb(34, 55, 75)), 1);
            editor.TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
        }

        public static void SetEnableNiceStyle(DependencyObject element, bool value)
        {
            element.SetValue(EnableNiceStyleProperty, value);
        }

        public static bool GetEnableNiceStyle(DependencyObject element) => (bool) element.GetValue(EnableNiceStyleProperty);
    }
}