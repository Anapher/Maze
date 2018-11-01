using System.Windows;
using Autofac;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceViewProvider
    {
        int Priority { get; }
        UIElement GetView(object viewModel, IComponentContext context);
    }
}