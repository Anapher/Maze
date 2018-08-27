using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace FileExplorer.Administration.Controls.Models
{
    public class AsyncAutoSuggestSource : ISuggestSource
    {
        public async Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            if (helper == null)
                return ImmutableList<object>.Empty;

            var valuePath = helper.ExtractPath(input);
            var valueName = helper.ExtractName(input);
            if (string.IsNullOrEmpty(valueName) && input.EndsWith(helper.Separator + ""))
                valueName += helper.Separator;

            if (valuePath == "" && input.EndsWith("" + helper.Separator))
                valuePath = valueName;
            var found = await helper.GetItemAsync(data, valuePath);

            if (found == null) return ImmutableList<object>.Empty;

            var result = new List<object>();
            var enuma = await helper.ListAsync(found);
            foreach (var item in enuma)
            {
                string valuePathName = helper.GetPath(item);
                if (valuePathName.StartsWith(input, helper.StringComparisonOption) &&
                    !valuePathName.Equals(input, helper.StringComparisonOption))
                    result.Add(item);
            }

            return result;
        }
    }
}