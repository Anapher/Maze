using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.PackageExtraction;
using NuGet.Packaging.Signing;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Extensions;
using Orcus.Server.Service.Modules.Extensions;

namespace Orcus.Server.Service.Modules
{
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
            if (_project.PrimaryPackages.Any(x => x.Equals(packageIdentity)))
                throw new InvalidOperationException($"Package '{packageIdentity}' is already installed");

            if (resolutionContext.DependencyBehavior == DependencyBehavior.Ignore)
                return ResolvedAction.CreateInstall(packageIdentity, _project.Sources).Yield();

            var resultingPackages = new HashSet<SourcedPackageIdentity>(_project.PrimaryPackages, PackageIdentity.Comparer);

            bool downgradeAllowed = false;

            //the final packages after the install completed
            var installedPackageWithSameId = resultingPackages.FirstOrDefault(x => x.IsSameId(packageIdentity));
            if (installedPackageWithSameId != null) //module is already installed in a different version
            {
                resultingPackages.Remove(installedPackageWithSameId);

                if (installedPackageWithSameId.Version > packageIdentity.Version)
                    downgradeAllowed = true; //if the installed version is greater, we must allow downgrades
            }

            resultingPackages.Add(packageIdentity);

            var requiredPackages = await GetRequiredPackages(resultingPackages, new List<PackageIdentity> {packageIdentity},
                downgradeAllowed, resolutionContext, _project.Framework, logger, token);
            return GetRequiredActions(requiredPackages);
        }

        public Task<IEnumerable<ResolvedAction>> PreviewUpdatePackagesAsync(List<SourcedPackageIdentity> packageIdentities, ResolutionContext resolutionContext, ILogger logger,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ResolvedAction>> PreviewDeletePackagesAsync(List<PackageIdentity> packageIdentities, ResolutionContext resolutionContext, ILogger logger,
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private async Task<PackagesContext> GetRequiredPackages(ISet<SourcedPackageIdentity> primaryPackages, IList<PackageIdentity> targetPackages,
            bool downgradeAllowed, ResolutionContext resolutionContext, NuGetFramework framework, ILogger logger, CancellationToken token)
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
                PrimaryTargets = targetPackages.ToList(),
                PrimaryTargetIds = primaryPackages.Where(x => !targetPackages.Contains(x)).Select(x => x.Id).ToList(),
                TargetFramework = framework,
                PrimarySources = _project.Sources,
                AllSources = _project.Sources,
                PackagesFolderSource = _project.LocalSourceRepository,
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
                preferredVersions: primaryPackages,
                availablePackages: prunedAvailablePackages,
                packageSources: _project.Sources.Select(x => x.PackageSource),
                log: logger);

            var packageResolver = new PackageResolver();

            //all final packages (including dependencies)
            var packages = packageResolver.Resolve(resolverContext, token).ToList(); //that's an array
            return new PackagesContext(packages, availablePackages);
        }

        private IEnumerable<ResolvedAction> GetRequiredActions(PackagesContext context)
        {
            var packagesToUninstall = new HashSet<PackageIdentity>(_project.InstalledPackages.Select(x => x.Key), PackageIdentity.Comparer);
            var packagesToInstall = new HashSet<PackageIdentity>(PackageIdentity.Comparer);

            foreach (var packageIdentity in context.Packages)
            {
                if (packagesToUninstall.Contains(packageIdentity))
                    packagesToUninstall.Remove(packageIdentity);
                else
                    packagesToInstall.Add(packageIdentity);
            }

            var actions = new List<ResolvedAction>(packagesToUninstall.Select(ResolvedAction.CreateUninstall));
            foreach (var packageIdentity in packagesToInstall)
                actions.Add(ResolvedAction.CreateInstall(packageIdentity, context.PackageDependencies[packageIdentity].Source));

            return actions;
        }

        public async Task ExecuteActionsAsync(IEnumerable<ResolvedAction> actions, PackageDownloadContext downloadContext, CancellationToken token)
        {
            var packageWithDirectoriesToBeDeleted = new HashSet<PackageIdentity>(PackageIdentity.Comparer);

            Dictionary<PackageIdentity, PackagePreFetcherResult> downloadTasks = null;
            var executedActions = new Stack<ResolvedAction>();
            ExceptionDispatchInfo exceptionInfo = null;
            CancellationTokenSource downloadTokenSource = null;

            try
            {
                var actionsList = actions.ToList();
                var hasInstalls = actionsList.Any(action => action.Action == ResolvedActionType.Install);

                if (hasInstalls)
                {
                    // Make this independently cancelable.
                    downloadTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

                    // Download all packages up front in parallel
                    downloadTasks = await PackageLoader.GetPackagesAsync(
                        actionsList,
                        _project.ModulesDirectory,
                        downloadContext,
                        NullLogger.Instance,
                        downloadTokenSource.Token);
                }

                foreach (var action in actionsList)
                {
                    if (action.Action == ResolvedActionType.Uninstall)
                    {
                        executedActions.Push(action);
                        await ExecuteUninstallAsync(action.PackageIdentity, packageWithDirectoriesToBeDeleted, token);
                    }
                }

                foreach (var nuGetProjectAction in actionsList)
                {
                    if (nuGetProjectAction.Action == ResolvedActionType.Install)
                    {
                        executedActions.Push(nuGetProjectAction);

                        // Retrieve the downloaded package
                        // This will wait on the package if it is still downloading
                        var preFetchResult = downloadTasks[nuGetProjectAction.PackageIdentity];
                        using (var downloadPackageResult = await preFetchResult.GetResultAsync())
                        {
                            // use the version exactly as specified in the nuspec file
                            var packageIdentity = await downloadPackageResult.PackageReader.GetIdentityAsync(token);

                            var signedPackageVerifier = new PackageSignatureVerifier(
                                SignatureVerificationProviderFactory.GetSignatureVerificationProviders(),
                                SignedPackageVerifierSettings.Default);

                            var packageExtractionContext = new PackageExtractionContext(
                                PackageSaveMode.Defaultv3,
                                PackageExtractionBehavior.XmlDocFileSaveMode,
                                NullLogger.Instance,
                                signedPackageVerifier);

                            downloadPackageResult.PackageStream.Position = 0;

                            await PackageExtractor.InstallFromSourceAsync(packageIdentity,
                                stream => downloadPackageResult.PackageStream.CopyToAsync(stream),
                                _project.ModulesDirectory.VersionFolderPathResolver, packageExtractionContext, token);

                            await ExecuteInstallAsync(packageIdentity, downloadPackageResult,
                                packageWithDirectoriesToBeDeleted, token);
                        }
                    }
                }
            }
            catch (SignatureException ex)
            {
                var errors = ex.Results.SelectMany(r => r.GetErrorIssues()).ToList();
                //var warnings = ex.Results.SelectMany(r => r.GetWarningIssues());
                SignatureException unwrappedException;

                if (errors.Count == 1)
                {
                    // In case of one error, throw it as the exception
                    var error = errors.First();
                    unwrappedException = new SignatureException(error.Code, error.Message, ex.PackageIdentity);
                }
                else
                {
                    // In case of multiple errors, wrap them in a general NU3000 error
                    var errorMessage = string.Format(CultureInfo.CurrentCulture,
                        "Signed package validation failed with multiple errors:{0}",
                        $"{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(e => e.FormatWithCode()))}");

                    unwrappedException = new SignatureException(NuGetLogCode.NU3000, errorMessage, ex.PackageIdentity);
                }

                exceptionInfo = ExceptionDispatchInfo.Capture(unwrappedException);
            }
            catch (Exception ex)
            {
                exceptionInfo = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                if (downloadTasks != null)
                {
                    // Wait for all downloads to cancel and dispose
                    downloadTokenSource.Cancel();

                    foreach (var result in downloadTasks.Values)
                    {
                        await result.EnsureResultAsync();
                        result.Dispose();
                    }

                    downloadTokenSource.Dispose();
                }
            }

            if (exceptionInfo != null)
            {
                await Rollback(executedActions, packageWithDirectoriesToBeDeleted, token);
            }

            // Delete the package directories as the last step, so that, if an uninstall had to be rolled back, we can just use the package file on the directory
            // Also, always perform deletion of package directories, even in a rollback, so that there are no stale package directories
            foreach (var packageWithDirectoryToBeDeleted in packageWithDirectoriesToBeDeleted)
            {
                try
                {
                    await _project.ModulesDirectory.DeleteModule(packageWithDirectoryToBeDeleted);
                }
                finally
                {
                    //if (DeleteOnRestartManager != null)
                    //{
                    //    if (Directory.Exists(packageFolderPath))
                    //    {
                    //        DeleteOnRestartManager.MarkPackageDirectoryForDeletion(
                    //            packageWithDirectoryToBeDeleted,
                    //            packageFolderPath,
                    //            nuGetProjectContext);

                    //        // Raise the event to notify listners to update the UI etc.
                    //        DeleteOnRestartManager.CheckAndRaisePackageDirectoriesMarkedForDeletion();
                    //    }
                    //}
                }
            }
        }

        private async Task Rollback(Stack<ResolvedAction> executedActions, ISet<PackageIdentity> packageWithDirectoriesToBeDeleted, CancellationToken token)
        {
            while (executedActions.Any())
            {
                var action = executedActions.Pop();
                try
                {
                    if (action.Action == ResolvedActionType.Install)
                    {
                        await ExecuteUninstallAsync(action.PackageIdentity, packageWithDirectoriesToBeDeleted, token);
                    }
                    else
                    {
                        packageWithDirectoriesToBeDeleted.Remove(action.PackageIdentity);
                        var packagePath =
                            _project.ModulesDirectory.VersionFolderPathResolver.GetPackageFilePath(
                                action.PackageIdentity.Id, action.PackageIdentity.Version);

                        if (File.Exists(packagePath))
                        {
                            using (var downloadResourceResult = new DownloadResourceResult(File.OpenRead(packagePath), action.SourceRepository?.PackageSource?.Source))
                            {
                                await ExecuteInstallAsync(action.PackageIdentity, downloadResourceResult, packageWithDirectoriesToBeDeleted, token);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //just ignore this exception, the NuGet team does the same (even if they aren't that sure too)
                }
            }
        }

        private async Task ExecuteUninstallAsync(PackageIdentity packageIdentity,
            ISet<PackageIdentity> packageWithDirectoriesToBeDeleted, CancellationToken token)
        {
            await _project.UninstallPackageAsync(packageIdentity, token);

            packageWithDirectoriesToBeDeleted.Add(packageIdentity);
        }

        private async Task ExecuteInstallAsync(PackageIdentity packageIdentity, DownloadResourceResult resourceResult,
            ISet<PackageIdentity> packageWithDirectoriesToBeDeleted, CancellationToken token)
        {
            await _project.InstallPackageAsync(packageIdentity, resourceResult, token);

            packageWithDirectoriesToBeDeleted.Remove(packageIdentity);
        }

        private async Task WriteModuleState(PackagesContext serverConext,
            ISet<SourcedPackageIdentity> primaryModules, ResolutionContext resolutionContext, ILogger logger, CancellationToken token)
        {
            var adminContext = await GetRequiredPackages(primaryModules, new List<PackageIdentity>(), 
                false, resolutionContext, OrcusFrameworks.Administration, logger, token);

            var clientContext = await GetRequiredPackages(primaryModules, new List<PackageIdentity>(),
                false, resolutionContext, OrcusFrameworks.Client, logger, token);

            await _project.SetModuleLock(primaryModules.ToList(), GetLock(serverConext), GetLock(adminContext),
                GetLock(clientContext));
        }

        private static IReadOnlyDictionary<PackageIdentity, IReadOnlyList<PackageIdentity>> GetLock(PackagesContext context)
        {
            return context.Packages.ToDictionary(x => x, x => (IReadOnlyList<PackageIdentity>) context
                .PackageDependencies[x].Dependencies
                .Select(y => context.Packages.First(z => z.Id == y.Id)).ToList());
        }

        private class PackagesContext
        {
            public PackagesContext(IReadOnlyCollection<PackageIdentity> packages, IReadOnlyCollection<SourcePackageDependencyInfo> sources)
            {
                Packages = packages;
                PackageDependencies = packages.ToDictionary(x => x, x => sources.First(y => y.Equals(x)),
                    PackageIdentity.Comparer);
            }

            public IEnumerable<PackageIdentity> Packages { get; }
            public IDictionary<PackageIdentity, SourcePackageDependencyInfo> PackageDependencies { get; }
        }
    }
}
