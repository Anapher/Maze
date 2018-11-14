using Anapher.Wpf.Swan.ViewInterface;
using Autofac;
using System;

namespace Orcus.Administration.Library.Views
{
    public interface IWindowService : IWindowInteractionService
    {
        bool? ShowDialog(Type viewModelType, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<object> configureViewModel, out object viewModel);
        void Show(Type viewModelType, Action<ContainerBuilder> configureContainer, Action<IShellWindow> configureWindow, Action<object> configureViewModel, out object viewModel);
    }
}
