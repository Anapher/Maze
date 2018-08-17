using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.PackageManagement;

namespace Orcus.Server.Service.Modules
{
    public interface IModulePackageManager
    {
        ResolutionContext GetDefaultResolutionContext();
        PackageDownloadContext GetDefaultDownloadContext();

        Task<IEnumerable<ResolvedAction>> PreviewInstallPackageAsync(PackageIdentity packageIdentity,
            ResolutionContext resolutionContext, CancellationToken token);

        Task<IEnumerable<ResolvedAction>> PreviewUpdatePackagesAsync(List<SourcedPackageIdentity> packageIdentities,
            ResolutionContext resolutionContext, CancellationToken token);

        Task<IEnumerable<ResolvedAction>> PreviewDeletePackagesAsync(List<PackageIdentity> packageIdentities,
            ResolutionContext resolutionContext, CancellationToken token);

        Task InstallPackageAsync(PackageIdentity packageIdentity, ResolutionContext resolutionContext,
            PackageDownloadContext downloadContext, CancellationToken token);

        Task<PackagesLock> GetPackagesLock(NuGetFramework framework);
    }
}