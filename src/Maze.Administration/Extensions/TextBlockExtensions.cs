using System;
using System.Windows;
using System.Windows.Controls;

namespace Maze.Administration.Extensions
{
    public static class TextBlockExtensions
    {
        public static readonly DependencyProperty LinesHeightProperty = DependencyProperty.RegisterAttached("LinesHeight", typeof(int),
            typeof(TextBlockExtensions), new PropertyMetadata(default(int), LinesHeightOnChanged));

        private static void LinesHeightOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = (TextBlock) d;

            var lineHeight = textBlock.LineHeight;
            if (double.IsNaN(lineHeight))
            {
                lineHeight = Math.Ceiling(textBlock.FontSize * textBlock.FontFamily.LineSpacing);
            }

            textBlock.Height = lineHeight * (int) e.NewValue;
        }

        public static void SetLinesHeight(DependencyObject element, int value)
        {
            element.SetValue(LinesHeightProperty, value);
        }

        public static int GetLinesHeight(DependencyObject element) => (int) element.GetValue(LinesHeightProperty);
    }
}