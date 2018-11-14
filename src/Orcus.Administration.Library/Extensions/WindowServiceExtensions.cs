using Autofac;
using Orcus.Administration.Library.Views;
using System;

namespace Orcus.Administration.Library.Extensions
{
    public static class WindowServiceExtensions
    {
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService)
        {
            return windowService.ShowDialog(typeof(TViewModel), null, null, null, out _);
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, null, null, out var vm);
            viewModel = (TViewModel) vm;
            return result;
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title)
        {
            return windowService.ShowDialog(typeof(TViewModel), null, window => window.Title = title, null, out _);
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, window => window.Title = title, null, out var vm);
            viewModel = (TViewModel) vm;
            return result;
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, Action<TViewModel> initializeViewModel)
        {
            return windowService.ShowDialog(typeof(TViewModel), null, null, x => initializeViewModel((TViewModel)x), out var vm);;
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, Action<TViewModel> initializeViewModel, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, null, x => initializeViewModel((TViewModel) x), out var vm);
            viewModel = (TViewModel)vm;

            return result;
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title, Action<TViewModel> initializeViewModel)
        {
            return windowService.ShowDialog(typeof(TViewModel), null, window => window.Title = title, x => initializeViewModel((TViewModel)x), out var vm); ;
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title, Action<TViewModel> initializeViewModel, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, window => window.Title = title, x => initializeViewModel((TViewModel)x), out var vm);
            viewModel = (TViewModel)vm;

            return result;
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<TViewModel> configureViewModel, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), configureContainer, configureWindow, x => configureViewModel((TViewModel)x), out var vm);
            viewModel = (TViewModel)vm;

            return result;
        }

        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, Action<IShellWindow> configureWindow, Action<TViewModel> configureViewModel, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, configureWindow, x => configureViewModel((TViewModel)x), out var vm);
            viewModel = (TViewModel)vm;

            return result;
        }

        public static void Show<TViewModel>(this IWindowService windowService)
        {
            windowService.Show(typeof(TViewModel), null, null , null, out _);
        }

        public static void Show<TViewModel>(this IWindowService windowService, string title)
        {
            windowService.Show(typeof(TViewModel), null, window => window.Title = title, null, out _);
        }

        public static void Show<TViewModel>(this IWindowService windowService, Action<TViewModel> initializeViewModel)
        {
            windowService.Show(typeof(TViewModel), null, null, x => initializeViewModel((TViewModel) x), out var vm);
        }

        public static void Show<TViewModel>(this IWindowService windowService, string title, Action<TViewModel> initializeViewModel)
        {
            windowService.Show(typeof(TViewModel), null, window => window.Title = title, x => initializeViewModel((TViewModel)x), out var vm);
        }

        public static void Show<TViewModel>(this IWindowService windowService, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<TViewModel> configureViewModel, out TViewModel viewModel)
        {
            windowService.Show(typeof(TViewModel), configureContainer, configureWindow, x => configureViewModel?.Invoke((TViewModel) x), out var vm);
            viewModel = (TViewModel)vm;
        }

        public static void Show<TViewModel>(this IWindowService windowService, Action<IShellWindow> configureWindow, Action<TViewModel> configureViewModel, out TViewModel viewModel)
        {
            windowService.Show(typeof(TViewModel), null, configureWindow, x => configureViewModel?.Invoke((TViewModel)x), out var vm);
            viewModel = (TViewModel)vm;
        }
    }
}