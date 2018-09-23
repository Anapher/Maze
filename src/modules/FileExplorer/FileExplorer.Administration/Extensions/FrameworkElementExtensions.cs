using System.Windows;
using System.Windows.Input;

namespace FileExplorer.Administration.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static readonly DependencyProperty SetClickHandledProperty = DependencyProperty.RegisterAttached("SetClickHandled", typeof(bool),
            typeof(FrameworkElementExtensions), new PropertyMetadata(default(bool), PropertyChangedCallback));

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.RegisterAttached("Progress", typeof(double),
            typeof(FrameworkElementExtensions), new PropertyMetadata(default(double)));

        public static void SetProgress(DependencyObject element, double value)
        {
            element.SetValue(ProgressProperty, value);
        }

        public static double GetProgress(DependencyObject element) => (double) element.GetValue(ProgressProperty);

        public static void SetSetClickHandled(DependencyObject element, bool value)
        {
            element.SetValue(SetClickHandledProperty, value);
        }

        public static bool GetSetClickHandled(DependencyObject element) => (bool) element.GetValue(SetClickHandledProperty);

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