using System.Windows;
using Autofac;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceViewProvider
    {
        UIElement GetView(object viewModel, IComponentContext context);
    }
}