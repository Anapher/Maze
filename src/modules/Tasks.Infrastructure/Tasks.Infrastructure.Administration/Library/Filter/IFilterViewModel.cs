using Tasks.Infrastructure.Core.Filter;

namespace Tasks.Infrastructure.Administration.Library.Filter
{
    public interface IFilterViewModel<TFilterInfo> : ITaskServiceViewModel<TFilterInfo> where TFilterInfo : FilterInfo
    {
    }
}