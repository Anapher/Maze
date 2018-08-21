using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Library.Controls
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

        private static IWindowViewManager GetViewManager(DependencyObject d) => ((WindowUserControl) d).ViewManager;
    }
}