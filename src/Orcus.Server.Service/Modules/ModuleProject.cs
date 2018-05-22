using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Orcus.Server.Connection.Modules;

namespace Orcus.Server.Service.Modules
{
    public interface IModuleProject
    {
        NuGetFramework Framework { get; }
        IImmutableList<SourcedPackageIdentity> PrimaryPackages { get; }
        IImmutableDictionary<PackageIdentity, IList<PackageDependencyGroup>> InstalledPackages { get; }
        IImmutableList<SourceRepository> Sources { get; }
        SourceRepository LocalRepository { get; }

        AsyncLock BatchLock { get; }

        Task<bool> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult,
            CancellationToken token);

        Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, CancellationToken token);
    }

    public interface IModulePackageManager
    {
        Task<IEnumerable<ResolvedAction>> PreviewInstallPackageAsync(SourcedPackageIdentity packageIdentity,
            ILogger logger, CancellationToken token);

        Task<IEnumerable<ResolvedAction>> PreviewUpdatePackagesAsync(List<SourcedPackageIdentity> packageIdentities,
            ILogger logger, CancellationToken token);

        Task<IEnumerable<ResolvedAction>> PreviewDeletePackagesAsync(List<PackageIdentity> packageIdentities,
            ILogger logger, CancellationToken token);

        Task ExecuteActionsAsync(IEnumerable<ResolvedAction> actions, PackageDownloadContext context,
            CancellationToken token);
    }

    public class ModulePackageManager : IModulePackageManager
    {
        private readonly IModuleProject _project;

        public ModulePackageManager(IModuleProject project)
        {
            _project = project;
        }

        public async Task<IEnumerable<ResolvedAction>> PreviewInstallPackageAsync(SourcedPackageIdentity packageIdentity,
            ResolutionContext resolutionContext, ILogger logger, CancellationToken token)
        {
            var currentPrimaryPackages = _project.PrimaryPackages.Cast<PackageIdentity>().ToList();
            if (currentPrimaryPackages.Any(x => x.Equals(packageIdentity)))
                throw new InvalidOperationException($"Package '{packageIdentity}' is already installed");

            var actions = new List<ResolvedAction>();
            if (resolutionContext.DependencyBehavior == DependencyBehavior.Ignore)
            {
                actions.Add(ResolvedAction.CreateInstall(packageIdentity, _project.Sources));
                return actions;
            }


        }

        private async Task<IEnumerable<ResolvedAction>> ResolveActions(ISet<SourcedPackageIdentity> primaryPackages, IList<PackageIdentity> targetPackages,
            bool downgradeAllowed, ResolutionContext resolutionContext, ILogger logger, CancellationToken token)
        {
            /* ===> GatherContext
             * InstalledPackages    Just checked that these packages don't get downgraded if the flag is not set. Basically the primary packages
             * PrimaryTargets       The parent elements of all depdendencies that should be searched for
             * PrimarySources       Primary Targets must be found here
             * AllSources           Depdendencies can be found here
             * PackagesFolderSource Used to scan for existing packages to skip a network request
             * ResolutionContext    Basic information about the package resolution. Only the cache is used here (and the DependencyBehavior, but the whole class wouldn't make sense with DependencyBehavior.Ignore)
             * AllowDowngrades      Determine if the InstalledPackages may be downgraded
             * ProjectContext       Only used for logging
             * ========================================================================
             * Return               A very huge list of all versions of PrimaryTargets aswell as all versions of it's dependencies (recursive!)
             */

            var gatherContext = new GatherContext
            {
                InstalledPackages = primaryPackages.ToList(), //these should not be downgraded in case of a different version
                PrimaryTargets = primaryPackages.ToList(),
                TargetFramework = _project.Framework,
                PrimarySources = _project.Sources,
                AllSources = _project.Sources,
                PackagesFolderSource = _project.LocalRepository,
                ResolutionContext = resolutionContext,
                AllowDowngrades = downgradeAllowed,
                ProjectContext = new EmptyNuGetProjectContext()
            };

            var availablePackages = await ResolverGather.GatherAsync(gatherContext, token);
            if (!availablePackages.Any())
                throw new InvalidOperationException("UnableToGatherDependencyInfo");

            //available packages now contains all versions of the package and all versions of each depdendency (recursive)
            //we try to prune the results down to only what we would allow to be installed

            //1. remove all versions of the package we want to install except the version we actually want to install
            //   it is not a problem if other primary packages might need an update
            var prunedAvailablePackages =
                PrunePackageTreeExtensions.RemoveAllVersionsForIdExcept(availablePackages, targetPackages);

            //2. remove the downgrades of primary packages
            if (!downgradeAllowed)
                prunedAvailablePackages = PrunePackageTreeExtensions.PruneDowngrades(prunedAvailablePackages, targetPackages);

            //3. remove prereleases
            if (!resolutionContext.IncludePrerelease)
                prunedAvailablePackages = PrunePackageTree.PrunePreleaseForStableTargets(prunedAvailablePackages, primaryPackages, targetPackages);

            /* ===> PackageResolverContext
             * TargetIds            New packages to install or update. These will prefer the highest version.
             * RequiredPackageIds   The required packages (primary)
             * PackagesConfig       Only for logging
             * PreferredVersions    Preferred versions of each package. If the package does not exist here it will use the dependency behavior, or if it is a target the highest version will be used.
             * AvailablePackages    All available packages that should be sorted out
             * DependencyBehavior   The behavior for resolving a package version
             * PackageSources       Only for logging
             * Log                  Logger
             * ========================================================================
             * Return               The complete resulting list
             */

            var resolverContext = new PackageResolverContext(
                resolutionContext.DependencyBehavior,
                targetIds: targetPackages.Select(x => x.Id),
                requiredPackageIds: primaryPackages.Select(x => x.Id),
                packagesConfig: Enumerable.Empty<PackageReference>(),
                preferredVersions: Enumerable.Empty<PackageIdentity>(),
                availablePackages: prunedAvailablePackages,
                packageSources: _project.Sources.Select(x => x.PackageSource),
                log: logger);

            var packageResolver = new PackageResolver();

            //all final packages (including dependencies)
            var newListOfInstalledPackages = packageResolver.Resolve(resolverContext, token); //that's an array


        }

        public Task<IEnumerable<ResolvedAction>> PreviewUpdatePackagesAsync(List<SourcedPackageIdentity> packageIdentities, ILogger logger, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ResolvedAction>> PreviewDeletePackagesAsync(List<PackageIdentity> packageIdentities, ILogger logger, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteActionsAsync(IEnumerable<ResolvedAction> actions, PackageDownloadContext context, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
