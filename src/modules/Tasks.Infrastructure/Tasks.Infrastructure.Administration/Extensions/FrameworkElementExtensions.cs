using System.Windows;
using System.Windows.Controls;
using Maze.Administration.Library.Utilities;

namespace Tasks.Infrastructure.Administration.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static readonly DependencyProperty SupressBringIntoViewProperty = DependencyProperty.RegisterAttached("SupressBringIntoView",
            typeof(bool), typeof(FrameworkElementExtensions), new PropertyMetadata(default(bool), OnSupressBringIntoViewChanged));

        public static void SetSupressBringIntoView(DependencyObject element, bool value)
        {
            element.SetValue(SupressBringIntoViewProperty, value);
        }

        public static bool GetSupressBringIntoView(DependencyObject element) => (bool) element.GetValue(SupressBringIntoViewProperty);

        private static void OnSupressBringIntoViewChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var frameworkElement = (FrameworkElement) dependencyObject;
            frameworkElement.RequestBringIntoView -= FrameworkElementOnRequestBringIntoView; //prevent memory leak

            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
                frameworkElement.RequestBringIntoView += FrameworkElementOnRequestBringIntoView;
        }

        private static void FrameworkElementOnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs requestBringIntoViewEventArgs)
        {
            requestBringIntoViewEventArgs.Handled = true;
            var itemsControl = WpfExtensions.VisualUpwardSearch<ItemsControl>((FrameworkElement) sender);
            if (itemsControl != null)
            {
                var scrollViewer = WpfExtensions.GetDescendantByType<ScrollViewer>(itemsControl);
                if (scrollViewer != null)
                {
                    var item = (FrameworkElement) requestBringIntoViewEventArgs.TargetObject;
                    var relativePoint = item.TranslatePoint(new Point(0, 0), itemsControl);

                    var childTransform = item.TransformToAncestor(scrollViewer);
                    var rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), item.RenderSize));
                    //Check if the elements Rect intersects with that of the scrollviewer’s
                    var result = Rect.Intersect(new Rect(new Point(0, 0), scrollViewer.RenderSize), rectangle);
                    var invisible = result == Rect.Empty;

                    if (invisible)
                        scrollViewer.ScrollToVerticalOffset(relativePoint.Y);
                }
            }
        }
    }
}