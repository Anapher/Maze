using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.ModuleManagement;

namespace Orcus.Server.Service.Modules
{
    public interface IModuleProject
    {
        NuGetFramework Framework { get; }

        IImmutableList<PackageIdentity> PrimaryPackages { get; }
        IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> InstalledPackages { get; }

        IImmutableList<SourceRepository> PrimarySources { get; }
        IImmutableList<SourceRepository> DependencySources { get; }
        SourceRepository LocalSourceRepository { get; }
        IModulesDirectory ModulesDirectory { get; }

        Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult,
            CancellationToken token);

        Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, CancellationToken token);
        Task SetServerModulesLock(IReadOnlyList<PackageIdentity> primaryModules, PackagesLock serverLock);
        Task AddModulesLock(NuGetFramework framework, PackagesLock packagesLock);
    }
}