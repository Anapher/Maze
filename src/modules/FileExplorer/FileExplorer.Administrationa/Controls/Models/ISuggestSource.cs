using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileExplorer.Administration.Controls.Models
{
    public interface ISuggestSource
    {
        Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper);
    }
}