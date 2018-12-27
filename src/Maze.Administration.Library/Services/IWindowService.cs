using Anapher.Wpf.Swan.ViewInterface;
using Autofac;
using System;

namespace Orcus.Administration.Library.Views
{
    /// <summary>
    ///     Provides functions to interact with the user and show new windows.
    /// </summary>
    public interface IWindowService : IWindowInteractionService
    {
        /// <summary>
        ///     Opens a window and returns only when the newly opened window is closed.
        /// </summary>
        /// <param name="viewModelType">The type of the view model. This type does not have to be registered in the DI container.s</param>
        /// <param name="configureContainer">A callback to configure the sub container that is created for the window.</param>
        /// <param name="configureWindow">Configure the window.</param>
        /// <param name="configureViewModel">Configure the view model</param>
        /// <param name="viewModel">Get the created view model</param>
        /// <returns>A <see cref="Nullable{T}"/> value of type Boolean that specifies whether the activity was accepted (<code>true</code>) or canceled (<code>false</code>). The return value is the value of the DialogResult property before a window closes.</returns>
        bool? ShowDialog(Type viewModelType, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<object> configureViewModel, out object viewModel);

        /// <summary>
        ///     Opens a window and returns without waiting for the newly opened window to close.
        /// </summary>
        /// <param name="viewModelType">The type of the view model. This type does not have to be registered in the DI container.s</param>
        /// <param name="configureContainer">A callback to configure the sub container that is created for the window.</param>
        /// <param name="configureWindow">Configure the window.</param>
        /// <param name="configureViewModel">Configure the view model</param>
        /// <param name="viewModel">Get the created view model</param>
        void Show(Type viewModelType, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<object> configureViewModel, out object viewModel);
    }
}
