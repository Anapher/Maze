using Anapher.Wpf.Swan;
using Anapher.Wpf.Swan.ViewInterface;
using Autofac;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using Maze.Administration.Library.Services;
using Maze.Administration.Library.StatusBar;
using Maze.Administration.Library.Views;
using Maze.Administration.Prism;
using Prism;
using System;
using System.Windows;
using Prism.Regions;

namespace Maze.Administration.Services
{
    public class WindowService : IWindowService
    {
        private readonly IWindow _window;
        private readonly IViewModelResolver _viewModelResolver;
        private readonly IContainer _container;
        private readonly IShellWindowFactory _shellWindowFactory;

        public WindowService(IContainer container, IViewModelResolver viewModelResolver,
            IShellWindowFactory shellWindowFactory, IWindow window)
        {
            _window = window;
            _viewModelResolver = viewModelResolver;
            _container = container;
            _shellWindowFactory = shellWindowFactory;
        }

        private IShellWindow Initialize(Type viewModelType, Action<object> configureViewModel, Action<IShellWindow> configureWindow,
            Action<ContainerBuilder> setupContainer, out object viewModel)
        {
            var viewType = _viewModelResolver.ResolveViewType(viewModelType);
            var window = _shellWindowFactory.Create();

            StatusBarManager statusBar = null;
            var lifescope = _container.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(window).As<IWindow>().As<IShellWindow>();
                builder.Register(context => statusBar = new StatusBarManager()).As<IShellStatusBar>().SingleInstance();
                builder.RegisterType(viewType);
                builder.RegisterType(viewModelType);
                builder.RegisterType<WindowService>().As<IWindowService>().SingleInstance();

                setupContainer?.Invoke(builder);
            });

            viewModel = lifescope.Resolve(viewModelType);
            configureViewModel?.Invoke(viewModel);

            var view = (FrameworkElement) lifescope.Resolve(viewType);
            view.DataContext = viewModel;

            window.InitalizeContent(view, statusBar);
            SetupWindowClosed(window, viewModel, lifescope);

            if (double.IsNaN(window.Height) && double.IsNaN(window.Width))
                window.SizeToContent = SizeToContent.WidthAndHeight;
            else if (double.IsNaN(window.Height))
                window.SizeToContent = SizeToContent.Height;
            else if (double.IsNaN(window.Width))
                window.SizeToContent = SizeToContent.Width;

            if (viewModel is INavigationAware navigationAware)
            {
                void OnViewLoaded(object s, EventArgs e)
                {
                    navigationAware.OnNavigatedTo(null);
                    view.Loaded -= OnViewLoaded;
                }
                void OnWindowClosed(object s, EventArgs e)
                {
                    navigationAware.OnNavigatedFrom(null);
                    window.Closed -= OnWindowClosed;
                }

                view.Loaded += OnViewLoaded;
                window.Closed += OnWindowClosed;
            }

            configureWindow?.Invoke(window);
            return window;
        }

        private static void SetupWindowClosed(IShellWindow window, object viewModel, ILifetimeScope lifescope)
        {
            if (viewModel is IActiveAware activeAware)
            {
                window.Closed += (sender, args) => activeAware.IsActive = false;
            }

            window.Closed += (sender, args) => lifescope.Dispose();
        }

        public bool? ShowDialog(Type viewModelType, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<object> configureViewModel, out object viewModel)
        {
            var window = Initialize(viewModelType, configureViewModel, configureWindow, configureContainer, out viewModel);
            return window.ShowDialog(_window);
        }

        public void Show(Type viewModelType, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<object> configureViewModel, out object viewModel)
        {
            var window = Initialize(viewModelType, configureViewModel, configureWindow, configureContainer, out viewModel);
            window.Show(_window);
        }

        public bool? ShowDialog(VistaFileDialog fileDialog)
        {
            return fileDialog.ShowDialog(_window as Window);
        }

        public bool? ShowDialog(FileDialog fileDialog)
        {
            return fileDialog.ShowDialog(_window as Window);
        }

        public bool? ShowDialog(VistaFolderBrowserDialog folderDialog)
        {
            return folderDialog.ShowDialog(_window as Window);
        }

        public MessageBoxResult ShowMessageBox(string text, string caption, MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defResult, MessageBoxOptions options)
        {
            return MessageBoxEx.Show(_window as Window, text, caption, buttons, icon, defResult, options);
        }
    }
}