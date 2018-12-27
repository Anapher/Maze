using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Orcus.Administration.Library.Extensions
{
    public static class ControlExtensions
    {
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommand", typeof(ICommand), typeof(ControlExtensions),
                new PropertyMetadata(default(ICommand), OnDoubleClickCommandChanged));

        public static readonly DependencyProperty DoubleClickSetHandledProperty =
            DependencyProperty.RegisterAttached("DoubleClickSetHandled", typeof(bool), typeof(ControlExtensions),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("DoubleClickCommandParameter", typeof(object),
                typeof(ControlExtensions), new PropertyMetadata(default(object)));

        public static void SetDoubleClickSetHandled(DependencyObject element, bool value)
        {
            element.SetValue(DoubleClickSetHandledProperty, value);
        }

        public static bool GetDoubleClickSetHandled(DependencyObject element) =>
            (bool) element.GetValue(DoubleClickSetHandledProperty);

        private static void OnDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Control control))
                throw new ArgumentException(nameof(d));

            if (e.NewValue == null && e.OldValue != null)
                control.MouseDoubleClick -= ControlOnMouseDoubleClick;
            else
                control.MouseDoubleClick += ControlOnMouseDoubleClick;
        }

        private static void ControlOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                var control = (Control) sender;

                var command = GetDoubleClickCommand(control);
                var parameter = GetDoubleClickCommandParameter(control);

                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                    e.Handled = GetDoubleClickSetHandled(control);
                }
            }
        }

        public static void SetDoubleClickCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(DoubleClickCommandProperty, value);
        }

        public static ICommand GetDoubleClickCommand(DependencyObject element) =>
            (ICommand) element.GetValue(DoubleClickCommandProperty);

        public static void SetDoubleClickCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(DoubleClickCommandParameterProperty, value);
        }

        public static object GetDoubleClickCommandParameter(DependencyObject element) =>
            element.GetValue(DoubleClickCommandParameterProperty);
    }
}