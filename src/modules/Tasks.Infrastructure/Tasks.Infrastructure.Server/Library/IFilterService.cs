using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Filter;

namespace Tasks.Infrastructure.Server.Library
{
    public interface IFilterService<in TFilterInfo> where TFilterInfo : FilterInfo
    {
        Task<bool> IncludeClient(TFilterInfo filterInfo, int clientId);
    }
}