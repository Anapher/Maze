using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Orcus.Administration.Library.Utilities;

namespace FileExplorer.Administration.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static readonly DependencyProperty SupressBringIntoViewProperty =
            DependencyProperty.RegisterAttached("SupressBringIntoView", typeof(bool),
                typeof(FrameworkElementExtensions), new PropertyMetadata(default(bool), OnSupressBringIntoViewChanged));

        public static readonly DependencyProperty SetClickHandledProperty =
            DependencyProperty.RegisterAttached("SetClickHandled", typeof(bool), typeof(FrameworkElementExtensions),
                new PropertyMetadata(default(bool), PropertyChangedCallback));

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.RegisterAttached("Progress",
            typeof(double), typeof(FrameworkElementExtensions), new PropertyMetadata(default(double)));

        public static void SetProgress(DependencyObject element, double value)
        {
            element.SetValue(ProgressProperty, value);
        }

        public static double GetProgress(DependencyObject element) => (double) element.GetValue(ProgressProperty);

        public static void SetSupressBringIntoView(DependencyObject element, bool value)
        {
            element.SetValue(SupressBringIntoViewProperty, value);
        }

        public static bool GetSupressBringIntoView(DependencyObject element) =>
            (bool) element.GetValue(SupressBringIntoViewProperty);

        public static void SetSetClickHandled(DependencyObject element, bool value)
        {
            element.SetValue(SetClickHandledProperty, value);
        }

        public static bool GetSetClickHandled(DependencyObject element) =>
            (bool) element.GetValue(SetClickHandledProperty);

        private static void OnSupressBringIntoViewChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var frameworkElement = (FrameworkElement) dependencyObject;
            frameworkElement.RequestBringIntoView -= FrameworkElementOnRequestBringIntoView; //prevent memory leak

            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
                frameworkElement.RequestBringIntoView += FrameworkElementOnRequestBringIntoView;
        }

        private static void FrameworkElementOnRequestBringIntoView(object sender,
            RequestBringIntoViewEventArgs requestBringIntoViewEventArgs)
        {
            requestBringIntoViewEventArgs.Handled = true;
            var itemsControl = WpfExtensions.FindParent<ItemsControl>((FrameworkElement) sender);
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

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement) d;

            if (e.OldValue as bool? == true) frameworkElement.PreviewMouseDown -= FrameworkElementOnMouseDown;

            if (e.NewValue as bool? == true) frameworkElement.PreviewMouseDown += FrameworkElementOnMouseDown;
        }

        private static void FrameworkElementOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}