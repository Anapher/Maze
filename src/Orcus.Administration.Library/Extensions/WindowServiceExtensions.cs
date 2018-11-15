using Autofac;
using Orcus.Administration.Library.Views;
using System;

namespace Orcus.Administration.Library.Extensions
{
    /// <summary>
    ///     Extensions for <see cref="IWindowService"/>
    /// </summary>
    public static class WindowServiceExtensions
    {
        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService)
        {
            return windowService.ShowDialog(typeof(TViewModel), null, null, null, out _);
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="viewModel">The created view model instance of the window</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, null, null, out var vm);
            viewModel = (TViewModel) vm;
            return result;
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="title">The title of the window that will be opened</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title)
        {
            return windowService.ShowDialog(typeof(TViewModel), null, window => window.Title = title, null, out _);
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="title">The title of the window that will be opened</param>
        /// <param name="viewModel">The created view model instance of the window.</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, window => window.Title = title, null, out var vm);
            viewModel = (TViewModel) vm;
            return result;
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, Action<TViewModel> initializeViewModel)
        {
            return windowService.ShowDialog(typeof(TViewModel), null, null, x => initializeViewModel((TViewModel)x), out var vm);;
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        /// <param name="viewModel">The created view model instance of the window.</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, Action<TViewModel> initializeViewModel, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, null, x => initializeViewModel((TViewModel) x), out var vm);
            viewModel = (TViewModel)vm;

            return result;
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="title">The title of the window that will be opened</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title, Action<TViewModel> initializeViewModel)
        {
            return windowService.ShowDialog(typeof(TViewModel), null, window => window.Title = title, x => initializeViewModel((TViewModel)x), out var vm); ;
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="title">The title of the window that will be opened</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        /// <param name="viewModel">The created view model instance of the window.</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, string title, Action<TViewModel> initializeViewModel, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, window => window.Title = title, x => initializeViewModel((TViewModel)x), out var vm);
            viewModel = (TViewModel)vm;

            return result;
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="configureContainer">Initialize the Autofac container.</param>
        /// <param name="configureWindow">Initialize the window before it gets shown.</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        /// <param name="viewModel">The created view model instance of the window.</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<TViewModel> initializeViewModel, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), configureContainer, configureWindow, x => initializeViewModel((TViewModel)x), out var vm);
            viewModel = (TViewModel)vm;

            return result;
        }

        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="configureWindow">Initialize the window before it gets shown.</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        /// <param name="viewModel">The created view model instance of the window.</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        public static bool? ShowDialog<TViewModel>(this IWindowService windowService, Action<IShellWindow> configureWindow, Action<TViewModel> initializeViewModel, out TViewModel viewModel)
        {
            var result = windowService.ShowDialog(typeof(TViewModel), null, configureWindow, x => initializeViewModel((TViewModel)x), out var vm);
            viewModel = (TViewModel)vm;

            return result;
        }

        /// <summary>
        /// Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        public static void Show<TViewModel>(this IWindowService windowService)
        {
            windowService.Show(typeof(TViewModel), null, null , null, out _);
        }

        /// <summary>
        /// Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="title">The title of the window that will be opened</param>
        public static void Show<TViewModel>(this IWindowService windowService, string title)
        {
            windowService.Show(typeof(TViewModel), null, window => window.Title = title, null, out _);
        }

        /// <summary>
        /// Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        public static void Show<TViewModel>(this IWindowService windowService, Action<TViewModel> initializeViewModel)
        {
            windowService.Show(typeof(TViewModel), null, null, x => initializeViewModel((TViewModel) x), out var vm);
        }

        /// <summary>
        /// Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="title">The title of the window that will be opened</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        public static void Show<TViewModel>(this IWindowService windowService, string title, Action<TViewModel> initializeViewModel)
        {
            windowService.Show(typeof(TViewModel), null, window => window.Title = title, x => initializeViewModel((TViewModel)x), out var vm);
        }

        /// <summary>
        /// Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="configureContainer">Initialize the Autofac container.</param>
        /// <param name="configureWindow">Initialize the window before it gets shown.</param>
        /// <param name="initializeViewModel">A delegate to call methods on the <see cref="TViewModel"/> before the window will be shown for initialization.</param>
        /// <param name="viewModel">The created view model instance of the window.</param>
        public static void Show<TViewModel>(this IWindowService windowService, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<TViewModel> initializeViewModel, out TViewModel viewModel)
        {
            windowService.Show(typeof(TViewModel), configureContainer, configureWindow, x => initializeViewModel?.Invoke((TViewModel) x), out var vm);
            viewModel = (TViewModel)vm;
        }

        /// <summary>
        /// Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <typeparam name="TViewModel">The view model of the window</typeparam>
        /// <param name="windowService">The window service of the owning window.</param>
        /// <param name="configureWindow">Configure the window before it will be shown.</param>
        /// <param name="initializeViewModel">Configure the </param>
        /// <param name="viewModel">The created view model instance of the window.</param>
        public static void Show<TViewModel>(this IWindowService windowService, Action<IShellWindow> configureWindow, Action<TViewModel> initializeViewModel, out TViewModel viewModel)
        {
            windowService.Show(typeof(TViewModel), null, configureWindow, x => initializeViewModel?.Invoke((TViewModel)x), out var vm);
            viewModel = (TViewModel)vm;
        }
    }
}