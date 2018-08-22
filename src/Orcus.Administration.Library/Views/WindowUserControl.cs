using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace Orcus.Administration.Library.Views
{
    public class WindowUserControl : UserControl
    {
        public static readonly DependencyProperty EscapeClosesWindowProperty =
            DependencyProperty.Register("EscapeClosesWindow", typeof(bool), typeof(WindowUserControl),
                new PropertyMetadata(default(bool),
                    (o, args) => GetViewManager(o).EscapeClosesWindow = (bool) args.NewValue));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string),
            typeof(WindowUserControl),
            new PropertyMetadata(default(string), (o, args) => GetViewManager(o).Title = (string) args.NewValue));

        public static readonly DependencyProperty RightStatusBarContentProperty =
            DependencyProperty.Register("RightStatusBarContent", typeof(object), typeof(WindowUserControl),
                new PropertyMetadata(default, (o, args) => GetViewManager(o).RightStatusBarContent = args.NewValue));

        public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register(
            "LeftWindowCommands", typeof(WindowCommands), typeof(WindowUserControl),
            new PropertyMetadata(default(WindowCommands),
                (o, args) => GetViewManager(o).LeftWindowCommands = (WindowCommands) args.NewValue));

        public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register(
            "RightWindowCommands", typeof(WindowCommands), typeof(WindowUserControl),
            new PropertyMetadata(default(WindowCommands),
                (o, args) => GetViewManager(o).RightWindowCommands = (WindowCommands) args.NewValue));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object),
            typeof(WindowUserControl),
            new PropertyMetadata(default, (o, args) => GetViewManager(o).TitleBarIcon = args.NewValue));

        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register("Flyouts",
            typeof(FlyoutsControl), typeof(WindowUserControl),
            new PropertyMetadata(default(FlyoutsControl),
                (o, args) => GetViewManager(o).Flyouts = (FlyoutsControl) args.NewValue));

        protected readonly IWindowViewManager ViewManager;

        public WindowUserControl(IWindowViewManager viewManager)
        {
            ViewManager = viewManager;
        }

        private WindowUserControl()
        {
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public object RightStatusBarContent
        {
            get => GetValue(RightStatusBarContentProperty);
            set => SetValue(RightStatusBarContentProperty, value);
        }

        public WindowCommands LeftWindowCommands
        {
            get => (WindowCommands) GetValue(LeftWindowCommandsProperty);
            set => SetValue(LeftWindowCommandsProperty, value);
        }

        public WindowCommands RightWindowCommands
        {
            get => (WindowCommands) GetValue(RightWindowCommandsProperty);
            set => SetValue(RightWindowCommandsProperty, value);
        }

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public FlyoutsControl Flyouts
        {
            get => (FlyoutsControl) GetValue(FlyoutsProperty);
            set => SetValue(FlyoutsProperty, value);
        }

        public bool EscapeClosesWindow
        {
            get => (bool) GetValue(EscapeClosesWindowProperty);
            set => SetValue(EscapeClosesWindowProperty, value);
        }

        private static IWindowViewManager GetViewManager(DependencyObject d)
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return new DummyViewManager();
#endif
            return ((WindowUserControl) d).ViewManager;
        }

#if DEBUG
#pragma warning disable CS0067
        private class DummyViewManager : IWindowViewManager
        {
            public WindowState WindowState { get; set; }
            public event EventHandler Closed;
            public event CancelEventHandler Closing;
            public void Close()
            {
                throw new NotImplementedException();
            }

            public bool Activate() => throw new NotImplementedException();

            public MessageBoxResult ShowMessageBox(string text, string caption, MessageBoxButton buttons, MessageBoxImage icon,
                MessageBoxResult defResult, MessageBoxOptions options) =>
                throw new NotImplementedException();

            public bool? ShowDialog(VistaFileDialog fileDialog) => throw new NotImplementedException();

            public bool? ShowDialog(FileDialog fileDialog) => throw new NotImplementedException();

            public string Title { get; set; }
            public object RightStatusBarContent { get; set; }
            public bool EscapeClosesWindow { get; set; }
            public WindowCommands LeftWindowCommands { get; set; }
            public WindowCommands RightWindowCommands { get; set; }
            public object TitleBarIcon { get; set; }
            public FlyoutsControl Flyouts { get; set; }
        }
#endif
    }
}