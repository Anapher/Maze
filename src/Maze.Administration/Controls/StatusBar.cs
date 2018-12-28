using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Maze.Administration.Library.StatusBar;
using Maze.Administration.Library.Utilities;

namespace Maze.Administration.Controls
{
    public class StatusBar : Control
    {
        public static readonly DependencyProperty ShellStatusBarProperty = DependencyProperty.Register("ShellStatusBar",
            typeof(StatusBarManager), typeof(StatusBar),
            new PropertyMetadata(default(StatusBarManager), PropertyChangedCallback));

        public static readonly DependencyProperty CurrentModeProperty = DependencyProperty.Register("CurrentMode",
            typeof(StatusBarMode), typeof(StatusBar), new PropertyMetadata(default(StatusBarMode)));

        static StatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBar),
                new FrameworkPropertyMetadata(typeof(StatusBar)));
        }

        public StatusBarManager ShellStatusBar
        {
            get => (StatusBarManager) GetValue(ShellStatusBarProperty);
            set => SetValue(ShellStatusBarProperty, value);
        }

        public StatusBarMode CurrentMode
        {
            get => (StatusBarMode) GetValue(CurrentModeProperty);
            set => SetValue(CurrentModeProperty, value);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            void StatusBarOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                var statusBar = (StatusBar) d;
                var statusBarManager = (StatusBarManager) sender;

                statusBar.Dispatcher.InvokeIfRequired(() =>
                    statusBar.CurrentMode =
                        statusBarManager.CurrentStatusMessage?.StatusBarMode ?? StatusBarMode.Default);
            }

            if (args.OldValue != null) ((StatusBarManager) args.OldValue).PropertyChanged -= StatusBarOnPropertyChanged;

            if (args.NewValue != null) ((StatusBarManager) args.NewValue).PropertyChanged += StatusBarOnPropertyChanged;
        }
    }
}