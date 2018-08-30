using System;
using System.Windows;
using Anapher.Wpf.Swan.ViewInterface;
using Autofac;
using Orcus.Administration.Library.Services;
using Orcus.Administration.Library.StatusBar;
using Orcus.Administration.Library.Views;
using Orcus.Administration.Prism;
using Prism.Regions;
using Unclassified.TxLib;

namespace Orcus.Administration.Services
{
    public class WindowService : IWindowService
    {
        private readonly IComponentContext _componentContext;
        private readonly IViewModelResolver _viewModelResolver;
        private readonly IShellWindowFactory _shellWindowFactory;

        public WindowService(IComponentContext componentContext, IViewModelResolver viewModelResolver,
            IShellWindowFactory shellWindowFactory)
        {
            _componentContext = componentContext;
            _viewModelResolver = viewModelResolver;
            _shellWindowFactory = shellWindowFactory;
        }

        private IShellWindow Initialize(Type viewModelType, string titleResourceKey, Action<IShellWindow> configureWindow,
            Action<ContainerBuilder> setupContainer)
        {
            var viewType = _viewModelResolver.ResolveViewType(viewModelType);
            var window = _shellWindowFactory.Create();
            window.InitializeTitleBar(Tx.T(titleResourceKey), null);

            StatusBarManager statusBar = null;
            var lifescope = _componentContext.Resolve<ILifetimeScope>().BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(window.ViewManager).As<IWindow>().As<IDialogWindow>().As<IWindowViewManager>();
                builder.Register(context => statusBar = new StatusBarManager()).As<IShellStatusBar>().SingleInstance();
                builder.RegisterType(viewType);
                builder.RegisterType(viewModelType);

                setupContainer?.Invoke(builder);
            });

            var viewModel = lifescope.Resolve(viewModelType);
            var view = (FrameworkElement) lifescope.Resolve(viewType);
            view.DataContext = viewModel;

            window.InitalizeContent(view, statusBar);
            SetupLoadUnload(window, viewModel, lifescope);

            if (double.IsNaN(window.ViewManager.Height) && double.IsNaN(window.ViewManager.Width))
                window.Window.SizeToContent = SizeToContent.WidthAndHeight;
            else if (double.IsNaN(window.ViewManager.Height))
                window.Window.SizeToContent = SizeToContent.Height;
            else if (double.IsNaN(window.ViewManager.Width))
                window.Window.SizeToContent = SizeToContent.Width;

            configureWindow?.Invoke(window);
            return window;
        }

        public IShellWindow Initialize(object viewModel, string titleResourceKey, Action<IShellWindow> configureWindow,
            Action<ContainerBuilder> setupContainer)
        {
            var viewType = _viewModelResolver.ResolveViewType(viewModel.GetType());
            var window = _shellWindowFactory.Create();
            window.InitializeTitleBar(Tx.T(titleResourceKey), null);

            StatusBarManager statusBar = null;
            var lifescope = _componentContext.Resolve<ILifetimeScope>().BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(window.ViewManager).As<IWindow>().As<IDialogWindow>().As<IWindowViewManager>();
                builder.Register(context => statusBar = new StatusBarManager()).As<IShellStatusBar>().SingleInstance();
                builder.RegisterType(viewType);

                setupContainer?.Invoke(builder);
            });

            var view = (FrameworkElement) lifescope.Resolve(viewType);
            view.DataContext = viewModel;

            window.InitalizeContent(view, statusBar);
            SetupLoadUnload(window, viewModel, lifescope);

            if (double.IsNaN(window.ViewManager.Height) && double.IsNaN(window.ViewManager.Width))
                window.Window.SizeToContent = SizeToContent.WidthAndHeight;
            else if (double.IsNaN(window.ViewManager.Height))
                window.Window.SizeToContent = SizeToContent.Height;
            else if (double.IsNaN(window.ViewManager.Width))
                window.Window.SizeToContent = SizeToContent.Width;

            configureWindow?.Invoke(window);
            return window;
        }

        private static void SetupLoadUnload(IShellWindow window, object viewModel, ILifetimeScope lifescope)
        {
            if (viewModel is INavigationAware navigationAware)
                window.Window.Loaded += (sender, args) => navigationAware.OnNavigatedTo(null);

            window.Window.Closed += (sender, args) => lifescope.Dispose();
        }

        public void Show(Type viewModelType, string titleResourceKey, IWindow owner, Action<IShellWindow> configureWindow,
            Action<ContainerBuilder> setupContainer)
        {
            var window = Initialize(viewModelType, titleResourceKey, configureWindow, setupContainer);
            window.Show(owner);
        }

        public void Show(object viewModel, string titleResourceKey, IWindow owner, Action<IShellWindow> configureWindow,
            Action<ContainerBuilder> setupContainer)
        {
            var window = Initialize(viewModel, titleResourceKey, configureWindow, setupContainer);
            window.Show(owner);
        }

        public bool? ShowDialog(Type viewModelType, string titleResourceKey, IWindow owner, Action<IShellWindow> configureWindow,
            Action<ContainerBuilder> setupContainer)
        {
            var window = Initialize(viewModelType, titleResourceKey, configureWindow, setupContainer);
            window.Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return window.ShowDialog(owner);
        }

        public bool? ShowDialog(object viewModel, string titleResourceKey, IWindow owner, Action<IShellWindow> configureWindow,
            Action<ContainerBuilder> setupContainer)
        {
            var window = Initialize(viewModel, titleResourceKey, configureWindow, setupContainer);
            window.Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            return window.ShowDialog(owner);
        }
    }
}