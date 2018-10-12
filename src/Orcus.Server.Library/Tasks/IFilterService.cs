using System.Threading.Tasks;
using Orcus.Server.Connection.Tasks.Filter;

namespace Orcus.Server.Library.Tasks
{
    public interface IFilterService<in TFilterInfo> where TFilterInfo : FilterInfo
    {
        Task<bool> IncludeClient(TFilterInfo filterInfo, int clientId);
    }
}