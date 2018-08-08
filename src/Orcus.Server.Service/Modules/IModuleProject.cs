using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.ModuleManagement;
using Orcus.ModuleManagement.Loader;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.Config;

namespace Orcus.Server.Service.Modules
{
    public interface IModuleProject
    {
        /// <summary>
        ///     The framework of this project
        /// </summary>
        NuGetFramework Framework { get; }

        /// <summary>
        ///     The runtime
        /// </summary>
        Runtime Runtime { get; }

        /// <summary>
        ///     The architecture
        /// </summary>
        Architecture Architecture { get; }

        /// <summary>
        ///     The modules lock
        /// </summary>
        IModulesLock ModulesLock { get; }

        /// <summary>
        ///     The primary packages (root)
        /// </summary>
        IImmutableList<PackageIdentity> PrimaryPackages { get; }

        /// <summary>
        ///     All installed packages with their dependencies
        /// </summary>
        IImmutableDictionary<PackageIdentity, IImmutableList<PackageIdentity>> InstalledPackages { get; }

        /// <summary>
        ///     The primary source repositories (for <see cref="PrimaryPackages"/>)
        /// </summary>
        IImmutableList<SourceRepository> PrimarySources { get; }

        /// <summary>
        ///     The source repositories for dependencies
        /// </summary>
        IImmutableList<SourceRepository> DependencySources { get; }

        /// <summary>
        ///     The local source repository
        /// </summary>
        SourceRepository LocalSourceRepository { get; }

        /// <summary>
        /// The modules directory
        /// </summary>
        IModulesDirectory ModulesDirectory { get; }

        /// <summary>
        ///     The supported frameworks with their standard libraries
        /// </summary>
        IReadOnlyDictionary<NuGetFramework, PackageIdentity> FrameworkLibraries { get; }

        /// <summary>
        ///     Complete the installation of a package
        /// </summary>
        /// <param name="packageIdentity"></param>
        /// <param name="downloadResourceResult"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult,
            CancellationToken token);

        /// <summary>
        ///     Complete the uninstallation of a package
        /// </summary>
        /// <param name="packageIdentity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, CancellationToken token);

        /// <summary>
        ///     Replace the current modules lock and the primary modules
        /// </summary>
        /// <param name="primaryModules">The new primary modules</param>
        /// <param name="serverLock">The new lock for the current <see cref="Framework"/></param>
        Task SetServerModulesLock(IReadOnlyList<PackageIdentity> primaryModules, PackagesLock serverLock);

        /// <summary>
        ///     Add another package lock for a different <see cref="framework"/>
        /// </summary>
        /// <param name="framework">The framework the package lock is for</param>
        /// <param name="packagesLock">The package lock</param>
        Task AddModulesLock(NuGetFramework framework, PackagesLock packagesLock);
    }
}