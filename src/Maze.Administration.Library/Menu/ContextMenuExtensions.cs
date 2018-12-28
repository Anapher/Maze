using System.Collections;
using System.Windows;

namespace Maze.Administration.Library.Menu
{
    public static class ContextMenuExtensions
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached("SelectedItems", typeof(IList),
            typeof(ContextMenuExtensions), new PropertyMetadata(default(IList)));

        public static void SetSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        public static IList GetSelectedItems(DependencyObject element) => (IList) element.GetValue(SelectedItemsProperty);
    }
}