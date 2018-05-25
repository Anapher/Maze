using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Connection.Modules;

namespace Orcus.Server.Service.Modules
{
    public interface IModuleProject
    {
        NuGetFramework Framework { get; }
        IImmutableList<SourcedPackageIdentity> PrimaryPackages { get; }
        IImmutableDictionary<PackageIdentity, IList<PackageDependencyGroup>> InstalledPackages { get; }
        IImmutableList<SourceRepository> Sources { get; }
        IModulesDirectory ModulesDirectory { get; }

        AsyncLock BatchLock { get; }

        Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult,
            CancellationToken token);

        Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, CancellationToken token);
    }
}