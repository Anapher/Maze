using System;
using System.Globalization;
using System.Windows;

namespace Tasks.Infrastructure.Administration.Core
{
    public class DefaultViewProvider : IViewProviderForAll
    {
        public int Priority { get; } = 0;

        public UIElement GetView(object viewModel, IServiceProvider serviceProvider)
        {
            var viewModelType = viewModel.GetType();
            var viewType = ResolveViewType(viewModelType);
            if (viewType == null)
                return null;

            var view = serviceProvider.GetService(viewType);
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

            var fullyQualifiedName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelType.Assembly.FullName);
            return Type.GetType(fullyQualifiedName);
        }

        private static string TrimEnd(string input, string suffixToRemove, StringComparison comparisonType)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
                return input.Substring(0, input.Length - suffixToRemove.Length);
            return input;
        }
    }
}