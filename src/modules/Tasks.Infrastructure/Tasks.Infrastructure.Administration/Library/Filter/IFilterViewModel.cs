using Tasks.Infrastructure.Core.Filter;

namespace Tasks.Infrastructure.Administration.Library.Filter
{
    /// <summary>
    ///     The view model of a filter (based on a <see cref="FilterInfo"/> data transfer object)
    /// </summary>
    /// <typeparam name="TFilterInfo">The data transfer object of the filter</typeparam>
    public interface IFilterViewModel<TFilterInfo> : ITaskServiceViewModel<TFilterInfo> where TFilterInfo : FilterInfo
    {
    }
}