using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.ModuleManagement;
using Orcus.Server.Connection.Modules;

namespace Orcus.Server.Service.Modules
{
    public interface IModuleProject
    {
        NuGetFramework Framework { get; }

        IImmutableList<SourcedPackageIdentity> PrimaryPackages { get; }
        IImmutableDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> InstalledPackages { get; }

        IImmutableList<SourceRepository> PrimarySources { get; }
        IImmutableList<SourceRepository> AllSources { get; }
        SourceRepository LocalSourceRepository { get; }
        IModulesDirectory ModulesDirectory { get; }

        AsyncLock BatchLock { get; }

        Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult,
            CancellationToken token);

        Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, CancellationToken token);

        Task SetModuleLock(IReadOnlyList<SourcedPackageIdentity> primaryPackages,
            IReadOnlyDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> serverLock,
            IReadOnlyDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> adminLock,
            IReadOnlyDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> clientLock);
    }
}