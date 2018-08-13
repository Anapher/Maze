using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orcus.Administration.Prism
{
    public class ViewModelResolver
    {
        private readonly IReadOnlyDictionary<string, Type> _viewModelMap;

        public ViewModelResolver(Assembly viewModelsAssembly)
        {
            var types = viewModelsAssembly.GetExportedTypes();

            _viewModelMap = types.Where(x => x.Name.EndsWith("ViewModel")).ToDictionary(ExtractViewModelName, x => x);
        }

        public Type ResolveViewModelType(Type viewType)
        {
            var name = ExtractViewName(viewType);
            return _viewModelMap[name];
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