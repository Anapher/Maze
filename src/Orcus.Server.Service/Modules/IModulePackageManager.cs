using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.PackageManagement;

namespace Orcus.Server.Service.Modules
{
    public interface IModulePackageManager
    {
        Task<IEnumerable<ResolvedAction>> PreviewInstallPackageAsync(SourcedPackageIdentity packageIdentity,
            ResolutionContext resolutionContext, ILogger logger, CancellationToken token);

        Task<IEnumerable<ResolvedAction>> PreviewUpdatePackagesAsync(List<SourcedPackageIdentity> packageIdentities,
            ResolutionContext resolutionContext, ILogger logger, CancellationToken token);

        Task<IEnumerable<ResolvedAction>> PreviewDeletePackagesAsync(List<PackageIdentity> packageIdentities,
            ResolutionContext resolutionContext, ILogger logger, CancellationToken token);

        Task ExecuteActionsAsync(IEnumerable<ResolvedAction> actions, PackageDownloadContext downloadContext,
            CancellationToken token);
    }
}