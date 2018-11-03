using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using Autofac;
using Tasks.Infrastructure.Administration.Core;

namespace Tasks.Common.Administration.ViewProvider
{
    public class CommonViewProvider : IViewProviderForAll
    {
        private readonly Assembly _currentAssembly;

        public CommonViewProvider()
        {
            _currentAssembly = Assembly.GetExecutingAssembly();
        }

        public int Priority { get; set; } = 100;

        public UIElement GetView(object viewModel, IComponentContext context)
        {
            var viewModelType = viewModel.GetType();
            if (viewModelType.Assembly != _currentAssembly)
                return null;

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
            var namsp = viewModelType.Namespace;
            var lastIndex = namsp.LastIndexOf('.');
            var viewName = namsp.Substring(0, lastIndex) + ".Views" + namsp.Substring(lastIndex, namsp.Length - lastIndex) + "." +
                           TrimEnd(viewModelType.Name, "Model", StringComparison.OrdinalIgnoreCase);

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