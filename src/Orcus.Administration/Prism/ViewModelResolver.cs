using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Orcus.Administration.Prism
{
    public interface IViewModelResolver
    {
        Type ResolveViewModelType(Type viewType);
        Type ResolveViewType(Type viewModelType);
    }

    public class ViewModelResolver : IViewModelResolver
    {
        private readonly IReadOnlyDictionary<string, Type> _viewModelMap;
        private readonly Assembly _viewAssembly;

        public ViewModelResolver(Assembly viewModelsAssembly, Assembly viewAssembly)
        {
            _viewAssembly = viewAssembly;
            var types = viewModelsAssembly.GetExportedTypes();

            _viewModelMap = types.Where(x => x.Name.EndsWith("ViewModel")).ToDictionary(ExtractViewModelName, x => x);
        }

        public Type ResolveViewModelType(Type viewType)
        {
            var name = ExtractViewName(viewType);

            if (viewType.Assembly == _viewAssembly)
                return _viewModelMap[name];

            return DefaultResolve(viewType);
        }

        public Type ResolveViewType(Type viewModelType)
        {
            var viewName = TrimEnd(viewModelType.FullName, "Model", StringComparison.OrdinalIgnoreCase);
            viewName = viewName.Replace(".ViewModels.", ".Views.");

            var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelType.Assembly.FullName);
            return Type.GetType(viewModelName);
        }

        private static Type DefaultResolve(Type viewType)
        {
            var viewName = viewType.FullName;
            viewName = viewName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.Assembly.FullName;
            var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";

            var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
            return Type.GetType(viewModelName);
        }

        private static string ExtractViewModelName(Type type) =>
            TrimEnd(type.Name, "ViewModel", StringComparison.Ordinal);

        private static string ExtractViewName(Type type)
        {
            var name = type.Name;
            name = TrimEnd(name, "Window", StringComparison.Ordinal);
            name = TrimEnd(name, "View", StringComparison.Ordinal);

            return name;
        }

        private static string TrimEnd(string input, string suffixToRemove, StringComparison comparisonType)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
                return input.Substring(0, input.Length - suffixToRemove.Length);
            return input;
        }
    }
}