using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.PackageExtraction;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Orcus.ModuleManagement.PackageManagement;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.Extensions;
using Orcus.Server.Service.Modules.PackageManagement;
using Orcus.Server.Service.Modules.Resolution;

namespace Orcus.Server.Service.Modules
{
    public class ModulePackageManager : IModulePackageManager
    {
        private readonly IModuleProject _project;

        public ModulePackageManager(IModuleProject project)
        {
            _project = project;
        }

        public async Task<IEnumerable<ResolvedAction>> PreviewInstallPackageAsync(PackageIdentity packageIdentity,
            ResolutionContext resolutionContext, ILogger logger, CancellationToken token)
        {
            var (primaryPackages, downgradeAllowed) = GetPackageInstallContext(packageIdentity);
            var context = await GetRequiredPackages(primaryPackages, new HashSet<PackageIdentity> {packageIdentity},
                downgradeAllowed, resolutionContext, _project.Framework, logger, token);

            return GetRequiredActions(context);
        }

        public async Task InstallPackageAsync(PackageIdentity packageIdentity,
            ResolutionContext resolutionContext, PackageDownloadContext downloadContext, ILogger logger, CancellationToken token)
        {
            var (primaryPackages, downgradeAllowed) = GetPackageInstallContext(packageIdentity);
            
            //Server
            var context = await GetRequiredPackages(primaryPackages, new HashSet<PackageIdentity> {packageIdentity},
                downgradeAllowed, resolutionContext, _project.Framework, logger, token);
            
            var actions = GetRequiredActions(context);
            await ExecuteActionsAsync(actions, downloadContext, token);
            await WriteModuleState(context, primaryPackages);

            //Other frameworks
            foreach (var framework in _project.FrameworkLibraries.Keys.Where(x => x != _project.Framework))
            {
                context = await GetRequiredPackages(primaryPackages, new HashSet<PackageIdentity> {packageIdentity},
                    downgradeAllowed, resolutionContext, framework, logger, token);

                actions = GetRequiredActions(context);
                await ExecuteActionsAsync(actions, downloadContext, token);

                await _project.AddModulesLock(framework, GetLock(context));
            }
        }

        private (HashSet<PackageIdentity>, bool) GetPackageInstallContext(PackageIdentity packageIdentity)
        {
            if (_project.PrimaryPackages.Any(x => x.Equals(packageIdentity)))
                throw new InvalidOperationException($"Package '{packageIdentity}' is already installed");

            var resultingPackages = new HashSet<PackageIdentity>(_project.PrimaryPackages, PackageIdentity.Comparer);
            var downgradeAllowed = false;

            //the final packages after the install completed
            var installedPackageWithSameId = resultingPackages.FirstOrDefault(x => x.IsSameId(packageIdentity));
            if (installedPackageWithSameId != null) //module is already installed in a different version
            {
                resultingPackages.Remove(installedPackageWithSameId);

                if (installedPackageWithSameId.Version > packageIdentity.Version)
                    downgradeAllowed = true; //if the installed version is greater, we must allow downgrades
            }

            resultingPackages.Add(packageIdentity);
            return (resultingPackages, downgradeAllowed);
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

        private async Task<PackagesContext> GetRequiredPackages(ISet<PackageIdentity> primaryPackages,
            ISet<PackageIdentity> targetPackages, bool downgradeAllowed, ResolutionContext resolutionContext,
            NuGetFramework primaryFramework, ILogger logger, CancellationToken token)
        {
            var libraryPackage = GetFramworkLibrary(primaryFramework);

            var gatherContext = new GatherContext
            {
                ResolutionContext = resolutionContext,
                PrimarySources = _project.PrimarySources,
                DependencySources = _project.DependencySources,
                PackagesFolderSource = _project.LocalSourceRepository,
                PrimaryTargets =
                    primaryPackages
                        .Select(x => targetPackages.Contains(x) ? x : new PackageIdentity(x.Id, version: null))
                        .ToList(),
                AllowDowngrades = downgradeAllowed,
                PrimaryFramework = primaryFramework,
                DependencyFramework = OrcusFrameworks.MapToNetFramework(primaryFramework),
                Log = logger
            };

            var primaryPackageIds = primaryPackages.Select(x => x.Id).ToHashSet();

            var allPackages = await ResolverGather.GatherAsync(gatherContext, token);
            var availablePackages = allPackages.Where(x => !primaryPackageIds.Contains(x.Id) || x.Dependencies.Any()).ToList();
            if (!availablePackages.Any()) //packages not available for this framework
                return new PackagesContext(ImmutableDictionary<PackageIdentity, SourcePackageDependencyInfo>.Empty);

            //available packages now contains all versions of the package and all versions of each depdendency (recursive)
            //we try to prune the results down to only what we would allow to be installed

            //1. remove incorrect library packages
            var prunedAvailablePackages =
                PrunePackageTreeExtensions.RemoveLibraryPackage(availablePackages, libraryPackage);

            //2. remove all versions of the package we want to install except the version we actually want to install
            //   it is not a problem if other primary packages might need an update
            prunedAvailablePackages =
                PrunePackageTreeExtensions.RemoveAllVersionsForIdExcept(prunedAvailablePackages, targetPackages);

            //3. remove the downgrades of primary packages
            if (!downgradeAllowed)
                prunedAvailablePackages =
                    PrunePackageTreeExtensions.PruneDowngrades(prunedAvailablePackages, primaryPackages);

            //4. remove prereleases
            if (!resolutionContext.IncludePrerelease)
                prunedAvailablePackages =
                    PrunePackageTree.PrunePreleaseForStableTargets(prunedAvailablePackages, primaryPackages,
                        targetPackages);

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
                packageSources: Enumerable.Empty<PackageSource>(),
                log: logger);

            var packageResolver = new PackageResolver();

            //all final packages (including dependencies)
            var packages = packageResolver.Resolve(resolverContext, token).ToList(); //that's an array
            var dependencyMap = packages.ToDictionary(x => x, x => availablePackages.First(y => y.Equals(x)),
                PackageIdentity.Comparer);

            //remove library package and it's dependencies because they are always loaded
            var foundLibraryPackage = packages.FirstOrDefault(libraryPackage.IsSameId);
            if (foundLibraryPackage != null)
            {
                if (!foundLibraryPackage.Version.Equals(libraryPackage.Version))
                    throw new InvalidOperationException($"Invalid version of {libraryPackage} found: {foundLibraryPackage}");

                RemovePackage(foundLibraryPackage);

                //TODO remove package also from SourceDependencyInfo?

                void RemovePackage(PackageIdentity packageIdentity)
                {
                    var sourceInfo = dependencyMap[packageIdentity];
                    dependencyMap.Remove(packageIdentity);

                    foreach (var dependency in sourceInfo.Dependencies)
                    {
                        var package = dependencyMap.FirstOrDefault(x => x.Key.IsSameId(dependency)).Key;
                        if (package != null)
                            RemovePackage(package);
                    }
                }
            }

            return new PackagesContext(dependencyMap);
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
                    downloadTasks = await PackageLoader.GetPackagesAsync(actionsList, _project.ModulesDirectory,
                        downloadContext, NullLogger.Instance, downloadTokenSource.Token);
                }

                foreach (var action in actionsList)
                {
                    if (action.Action == ResolvedActionType.Uninstall)
                    {
                        executedActions.Push(action);
                        await ExecuteUninstallAsync(action.PackageIdentity, packageWithDirectoriesToBeDeleted, token);
                    }
                }

                foreach (var action in actionsList)
                {
                    if (action.Action == ResolvedActionType.Install)
                    {
                        executedActions.Push(action);

                        // Retrieve the downloaded package
                        // This will wait on the package if it is still downloading
                        var preFetchResult = downloadTasks[action.PackageIdentity];
                        using (var downloadPackageResult = await preFetchResult.GetResultAsync())
                        {
                            // use the version exactly as specified in the nuspec file
                            var packageIdentity = await downloadPackageResult.PackageReader.GetIdentityAsync(token);

                            var signedPackageVerifier = new PackageSignatureVerifier(
                                SignatureVerificationProviderFactory.GetSignatureVerificationProviders());

                            var packageExtractionContext = new PackageExtractionContext(
                                PackageSaveMode.Defaultv3,
                                PackageExtractionBehavior.XmlDocFileSaveMode,
                                NullLogger.Instance,
                                signedPackageVerifier,
                                SignedPackageVerifierSettings.GetDefault());

                            downloadPackageResult.PackageStream.Position = 0;

                            await PackageExtractor.InstallFromSourceAsync(action.SourceRepository.PackageSource.Source, packageIdentity,
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

        public async Task<PackagesLock> GetPackagesLock(NuGetFramework framework)
        {
            if (_project.ModulesLock.Modules.TryGetValue(framework, out var packagesLock))
                return packagesLock;

            throw new NotImplementedException(); //TODO create package lock
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

        private async Task WriteModuleState(PackagesContext serverContext,  IEnumerable<PackageIdentity> primaryModules)
        {
            await _project.SetServerModulesLock(primaryModules.ToList(), GetLock(serverContext));
        }

        private PackageIdentity GetFramworkLibrary(NuGetFramework framework)
        {
            if (_project.FrameworkLibraries.TryGetValue(framework, out var packageId))
                return packageId;

            var lowerFramework = _project.FrameworkLibraries.Keys
                .Where(x => x.Framework == framework.Framework && x.Version < framework.Version)
                .OrderByDescending(x => x.Version).FirstOrDefault();

            if (lowerFramework == null)
                throw new ArgumentException($"Framework {framework} not found");

            return _project.FrameworkLibraries[lowerFramework];
        }

        private static PackagesLock GetLock(PackagesContext context)
        {
            return new PackagesLock(context.Packages.ToImmutableDictionary(x => x,
                x => (IImmutableList<PackageIdentity>) context.PackageDependencies[x].Dependencies
                    .Select(y => context.Packages.FirstOrDefault(z => z.Id == y.Id)).Where(y => y != null)
                    .ToImmutableList()));
        }

        private class PackagesContext
        {
            public PackagesContext(IReadOnlyDictionary<PackageIdentity, SourcePackageDependencyInfo> packages)
            {
                Packages = packages.Select(x => x.Key).ToList();
                PackageDependencies = packages;
            }

            public IReadOnlyList<PackageIdentity> Packages { get; }
            public IReadOnlyDictionary<PackageIdentity, SourcePackageDependencyInfo> PackageDependencies { get; }
        }
    }
}
