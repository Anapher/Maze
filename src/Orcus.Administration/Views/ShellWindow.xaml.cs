using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Anapher.Wpf.Swan;
using Anapher.Wpf.Swan.Extensions;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using Orcus.Administration.Controls;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Views;

namespace Orcus.Administration.Views
{
    /// <summary>
    ///     Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : IWindowViewManager
    {
        private StatusBarManager _statusBarManager;
        private object _titleBarIcon;
        private object _rightStatusBarContent;

        public ShellWindow()
        {
            InitializeComponent();
            ShowIconOnTitleBar = false;
        }

        public void InitializeWindow(object content, StatusBarManager statusBarManager)
        {
            if (statusBarManager == null)
                Content = content;
            else
            {
                _statusBarManager = statusBarManager;
                statusBarManager.RightContent = _rightStatusBarContent;

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
                grid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});

                if (content is UIElement uiElement)
                {
                    Grid.SetRow(uiElement, 0);
                    grid.Children.Add(uiElement);
                }
                else
                {
                    var contentControl = new ContentControl {Content = content};
                    Grid.SetRow(contentControl, 0);
                    grid.Children.Add(contentControl);
                }

                var statusBar = new StatusBar {ShellStatusBar = statusBarManager};
                Grid.SetRow(statusBar, 1);
                grid.Children.Add(statusBar);

                Content = grid;
            }

            if (content is FrameworkElement fw)
                DataContext = fw.DataContext;
        }

        public MessageBoxResult ShowMessageBox(string text, string caption, MessageBoxButton buttons,
            MessageBoxImage icon, MessageBoxResult defResult, MessageBoxOptions options)
        {
            return MessageBoxEx.Show(this, text, caption, buttons, icon, defResult, options);
        }

        public object TitleBarIcon
        {
            get => _titleBarIcon;
            set
            {
                if (_titleBarIcon != value && value != null)
                {
                    if (Icon == null && value is ImageSource imageSource)
                        Icon = imageSource;
                    else
                    {
                        var factory = new FrameworkElementFactory(typeof(ContentPresenter));
                        factory.SetValue(ContentPresenter.ContentProperty, new Binding { Source = value });

                        IconTemplate = new DataTemplate { VisualTree = factory };
                        _titleBarIcon = value;
                    }

                    ShowIconOnTitleBar = true;
                }
            }
        }

        public ImageSource TaskBarIcon
        {
            get => Icon;
            set
            {
                Icon = value;
                ShowIconOnTitleBar = true;
            }
        }

        public object RightStatusBarContent
        {
            get => _statusBarManager?.RightContent;
            set
            {
                if (_statusBarManager != null)
                    _statusBarManager.RightContent = value;
                else _rightStatusBarContent = value;
            }
        }

        public bool EscapeClosesWindow
        {
            get => WindowService.GetEscapeClosesWindow(this);
            set => WindowService.SetEscapeClosesWindow(this, value);
        }

        public bool? ShowDialog(VistaFileDialog fileDialog)
        {
            return fileDialog.ShowDialog(this);
        }

        public bool? ShowDialog(FileDialog fileDialog)
        {
            return fileDialog.ShowDialog(this);
        }
    }
}