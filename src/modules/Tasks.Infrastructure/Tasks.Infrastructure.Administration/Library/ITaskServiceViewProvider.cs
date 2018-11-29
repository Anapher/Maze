using System.Windows;
using Autofac;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITaskServiceViewProvider
    {
        /// <summary>
        ///     The priority of this view provider. The greater this value the earlier it will be executed.
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     Get a view for a given view model
        /// </summary>
        /// <param name="viewModel">The view model that required a view</param>
        /// <param name="context">The component context to resolve services or views</param>
        /// <returns>Return the view for the given <see cref="viewModel"/>. If null, the next <see cref="ITaskServiceViewProvider"/> will be executed.</returns>
        UIElement GetView(object viewModel, IComponentContext context);
    }
}