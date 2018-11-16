using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Filter;

namespace Tasks.Infrastructure.Server.Library
{
    /// <summary>
    ///     The service for <see cref="FilterInfo"/>
    /// </summary>
    /// <typeparam name="TFilterInfo">The data transfer object for this service</typeparam>
    public interface IFilterService<in TFilterInfo> where TFilterInfo : FilterInfo
    {
        /// <summary>
        ///     Invoke the filter on a client
        /// </summary>
        /// <param name="filterInfo">The filter information</param>
        /// <param name="clientId">The client id of the client that should be decided</param>
        /// <returns>Return true if the task should execute on this client, and return false if this client should be skipped.</returns>
        Task<bool> IncludeClient(TFilterInfo filterInfo, int clientId);
    }
}