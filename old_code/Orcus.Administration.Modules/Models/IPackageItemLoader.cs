using System;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;

namespace Orcus.Administration.Modules.Models
{
    /// <summary>
    /// This enhance IItemLoader by adding package specific methods.
    /// </summary>
    public interface IPackageItemLoader : IItemLoader<PackageItemListViewModel>
    {
        Task<SearchResult<IPackageSearchMetadata>> SearchAsync(ContinuationToken continuationToken,
            CancellationToken cancellationToken);

        Task UpdateStateAndReportAsync(SearchResult<IPackageSearchMetadata> searchResult,
            IProgress<IItemLoaderState> progress, CancellationToken cancellationToken);
    }
}