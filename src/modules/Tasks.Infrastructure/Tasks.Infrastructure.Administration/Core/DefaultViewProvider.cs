using System;
using System.Globalization;
using System.Windows;
using Autofac;
using Tasks.Infrastructure.Administration.Library.Command;

namespace Tasks.Infrastructure.Administration.Core
{
    public class DefaultViewProvider : ICommandViewProvider
    {
        public int Priority { get; } = 0;

        public UIElement GetView(object viewModel, IComponentContext context)
        {
            var viewModelType = viewModel.GetType();
            var viewType = ResolveViewType(viewModelType);
            if (viewType == null)
                return null;

            var view = context.ResolveOptional(viewType);
            if (view == null)
                return null;

            if (view is FrameworkElement frameworkElement)
                frameworkElement.DataContext = viewModel;

            return view as UIElement;
        }

        public Type ResolveViewType(Type viewModelType)
        {
            var viewName = TrimEnd(viewModelType.FullName, "Model", StringComparison.OrdinalIgnoreCase);
            viewName = viewName.Replace(".ViewModels.", ".Views.");

            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelType.Assembly.FullName);
            return Type.GetType(viewModelName);
        }

        private static string TrimEnd(string input, string suffixToRemove, StringComparison comparisonType)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
                return input.Substring(0, input.Length - suffixToRemove.Length);
            return input;
        }
    }
}