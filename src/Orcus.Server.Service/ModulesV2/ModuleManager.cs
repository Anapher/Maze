using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.ModulesV2.Config;
using Orcus.Server.Service.ModulesV2.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace Orcus.Server.Service.ModulesV2
{
    public class ModulesOptions
    {
        /// <summary>
        ///     The directory where the module packages are found
        /// </summary>
        public string ModulesDirectory { get; set; }

        /// <summary>
        ///     The directory where the config of the modules should be stored
        /// </summary>
        public string ConfigDirectory { get; set; }

        /// <summary>
        ///     The directory that contains the unpacked modules from <see cref="ModulesDirectory" />
        /// </summary>
        public string CacheDirectory { get; set; }

        /// <summary>
        ///     Alternative repository sources for modules
        /// </summary>
        public List<Uri> AlternativeRepositories { get; set; }

        /// <summary>
        ///     The configuration of modules
        /// </summary>
        public IModulesConfig ModulesConfig { get; set; }

        /// <summary>
        ///     The configuration file that stores information about module dependencies
        /// </summary>
        public IModulesLock ModulesLock { get; set; }
    }



    public class ModulePackageManager
    {
        private readonly NuGetFramework _framework;
        private readonly IModuleApplicationService _moduleApplicationService;
        private readonly IReadOnlyList<SourceRepository> _sources;

        public ModulePackageManager(IModuleApplicationService moduleApplicationService,
            IReadOnlyList<SourceRepository> sources, NuGetFramework framework)
        {
            _moduleApplicationService = moduleApplicationService;
            _sources = sources;
        }

        public DependencyBehavior DependencyBehavior { get; set; }
        public bool IncludePrerelease { get; set; }

        public async Task<IEnumerable<ResolvedAction>> PreviewInstallPackageAsync(
            SourcedPackageIdentity packageIdentity, ILogger logger, CancellationToken token)
        {
            var currentPrimaryPackages = _moduleApplicationService.PrimaryModules.Cast<PackageIdentity>().ToList();
            if (currentPrimaryPackages.Any(x => x.Equals(packageIdentity)))
                throw new InvalidOperationException($"Package '{packageIdentity}' already exists");

            var actions = new List<ResolvedAction>();
            if (DependencyBehavior != DependencyBehavior.Ignore)
            {
                var downgradeAllowed = false;

                //the final packages after the install completed
                var resultingPackages = new HashSet<PackageIdentity>(currentPrimaryPackages, PackageIdentity.Comparer);

                var installedPackageWithSameId =
                    resultingPackages.FirstOrDefault(x => x.IsSameId(packageIdentity));
                if (installedPackageWithSameId != null) //module is already installed in a different version
                {
                    resultingPackages.Remove(installedPackageWithSameId);

                    if (installedPackageWithSameId.Version > packageIdentity.Version)
                        downgradeAllowed = true; //if the installed version is greater, we must allow downgrades
                }

                resultingPackages.Add(packageIdentity);

                logger.LogInformation("Attempting To Gather Depdendency Info");

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
                    InstalledPackages =
                        currentPrimaryPackages, //these should not be downgraded in case of a different version
                    PrimaryTargets = resultingPackages.ToList(),
                    TargetFramework = _moduleApplicationService.Framework,
                    PrimarySources = _sources,
                    AllSources = _sources,
                    PackagesFolderSource = _moduleApplicationService.LocalRepository,
                    ResolutionContext =
                        new ResolutionContext(DependencyBehavior, IncludePrerelease, true, VersionConstraints.None),
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
                    PrunePackageTree.RemoveAllVersionsForIdExcept(availablePackages, packageIdentity);

                //2. remove the downgrades of primary packages
                if (!downgradeAllowed)
                    prunedAvailablePackages = PrunePackageTree.PruneDowngrades(prunedAvailablePackages,
                        _moduleApplicationService.PrimaryModules.Select(x =>
                            new PackageReference(x, _moduleApplicationService.Framework)));

                //3. remove prereleases
                if (!IncludePrerelease)
                    prunedAvailablePackages = PrunePackageTree.PrunePreleaseForStableTargets(prunedAvailablePackages,
                        resultingPackages, new[] {packageIdentity});

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
                 * Return               The complete resuling list
                 */

                var resolverContext = new PackageResolverContext(
                    DependencyBehavior,
                    new[] {packageIdentity.Id},
                    currentPrimaryPackages.Select(x => x.Id),
                    Enumerable.Empty<PackageReference>(),
                    Enumerable.Empty<PackageIdentity>(),
                    prunedAvailablePackages,
                    _sources.Select(x => x.PackageSource),
                    logger);

                var packageResolver = new PackageResolver();

                //all final packages (including dependencies)
                var newListOfInstalledPackages = packageResolver.Resolve(resolverContext, token); //that's an array

                if (newListOfInstalledPackages == null)
                    throw new InvalidOperationException("UnableToResolveDependencyInfo");

                var allInstalledModules = _moduleApplicationService.InstalledModules;

                var packagesToUninstall = new List<PackageIdentity>();

                //loop through installed modules and check which of them do not exist in the new list (and uninstall them)
                foreach (var oldInstalledPackage in allInstalledModules)
                {
                    var newPackageWithSameId =
                        newListOfInstalledPackages.FirstOrDefault(np =>
                            np.IsSameId(oldInstalledPackage) && !np.Version.Equals(oldInstalledPackage.Version));

                    if (newPackageWithSameId != null)
                        packagesToUninstall.Add(oldInstalledPackage);
                }

                foreach (var newPackageToUninstall in packagesToUninstall)
                    actions.Add(ResolvedAction.CreateUninstall(newPackageToUninstall));

                var newPackagesToInstall = newListOfInstalledPackages.Where(p => !allInstalledModules.Contains(p));
                foreach (var package in newPackagesToInstall)
                {
                    var source = availablePackages.SingleOrDefault(x => x.Equals(package));
                    if (source == null)
                        throw new InvalidOperationException("PackageNotFound");

                    actions.Add(ResolvedAction.CreateInstall(source, source.Source));
                }
            }
            else
            {
                actions.Add(ResolvedAction.CreateInstall(packageIdentity, _sources));
            }

            return actions;
        }

        public async Task InstallPackageAsync(SourcedPackageIdentity packageIdentity, ILogger logger,
            CancellationToken token)
        {
            var actions = await PreviewInstallPackageAsync(packageIdentity, logger, token);
        }

        private async Task ExecuteActionsAsync(IEnumerable<ResolvedAction> actions,
            PackageDownloadContext downloadContext, CancellationToken token)
        {
            var executedActions = new Stack<NuGetProjectAction>();
            var packageWithDirectoriesToBeDeleted = new HashSet<PackageIdentity>(PackageIdentity.Comparer);

            Dictionary<PackageIdentity, PackagePreFetcherResult> downloadTasks = null;
            CancellationTokenSource downloadTokenSource = null;
            var executedNuGetProjectActions = new Stack<ResolvedAction>();


            var actionsList = actions.ToList();
            var hasInstalls = actionsList.Any(action => action.Action == ResolvedActionType.Install);

            if (hasInstalls)
            {
                // Make this independently cancelable.
                downloadTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

                // Download all packages up front in parallel
                downloadTasks = await PackageLoader.GetPackagesAsync(
                    actionsList,
                    _moduleApplicationService,
                    downloadContext,
                    NullLogger.Instance,
                    downloadTokenSource.Token);
            }

            foreach (var action in actionsList)
            {
                if (action.Action == ResolvedActionType.Uninstall)
                {
                    executedNuGetProjectActions.Push(action);
                    await _moduleApplicationService.ModulesConfig.Remove(action.PackageIdentity);

                    await ExecuteUninstallAsync(action.PackageIdentity, packageWithDirectoriesToBeDeleted);
                }
            }

            foreach (var nuGetProjectAction in actionsList)
            {
                if (nuGetProjectAction.Action == ResolvedActionType.Install)
                {
                    executedNuGetProjectActions.Push(nuGetProjectAction);

                    // Retrieve the downloaded package
                    // This will wait on the package if it is still downloading
                    var preFetchResult = downloadTasks[nuGetProjectAction.PackageIdentity];
                    using (var downloadPackageResult = await preFetchResult.GetResultAsync())
                    {
                        // use the version exactly as specified in the nuspec file
                        var packageIdentity = await downloadPackageResult.PackageReader.GetIdentityAsync(token);
                        packageWithDirectoriesToBeDeleted.Remove(packageIdentity);


                    }
                }
            }
        }

        private async Task ExecuteInstallAsync(PackageIdentity packageIdentity, bool isPrimaryModule)
        {
            var existingPackage =
                _moduleApplicationService.ModulesConfig.Modules.FirstOrDefault(x => x.IsSameId(packageIdentity));
            if (existingPackage != nu)
        }

        private Task ExecuteUninstallAsync(PackageIdentity packageIdentity)
        {
            return _moduleApplicationService.ModulesConfig.Remove(packageIdentity);
        }

        //public IEnumerable<ResolvedAction> InstallModuleAsync(SourcedPackageIdentity module)
        //{
        //    var actions = new List<ResolvedAction>();
        //    var finalModules = _applicationModules.PrimaryModules.ToList();

        //    foreach (var module in modules)
        //    {
        //        if (_applicationModules.PrimaryModules.Any(x => x.Equals(module)))
        //            continue; //module is installed and a primary module

        //        actions.Add(ResolvedAction.CreateInstall(module, _sources));
        //        finalModules.Add(module);

        //        if (_applicationModules.InstalledModules.Any(x => x.Equals(module)))
        //        {
        //            //module is already installed, but not primary. No need to resolve dependencies.
        //            //nothing must be changed except the config file
        //            continue;
        //        }

        //        var existingModule = _applicationModules.InstalledModules.FirstOrDefault(x => x.IsSameId(module));
        //        if (existingModule != null) //module is already installed in a different version, uninstall
        //        {
        //            //Upgrade!
        //            actions.Add(ResolvedAction.CreateUninstall(existingModule, _sources));
        //            finalModules.Remove(existingModule); //TODO: correct comparision?
        //        }

        //        //that's a completely new module
        //    }

        //    if(dependencyScanRequired)
        //}
    }

    public interface IModuleApplicationService
    {
        /// <summary>
        ///     The modules of the config file
        /// </summary>
        IModulesConfig ModulesConfig { get; }

        IModulesLock ModulesLock { get; }

        IImmutableList<SourcedPackageIdentity> PrimaryModules { get; }

        /// <summary>
        ///     All installed modules
        /// </summary>
        IImmutableList<SourcedPackageIdentity> InstalledModules { get; }

        NuGetFramework Framework { get; }

        SourceRepository LocalRepository { get; }

        string GetLocalPackagePath(PackageIdentity packageIdentity);
    }

    public class ModuleManager
    {
        private readonly IModuleLoader _moduleLoader;
        private readonly ModulesOptions _options;

        public ModuleManager(ModulesOptions options, IModuleLoader moduleLoader)
        {
            _options = options;
            _moduleLoader = moduleLoader;
        }

        public async Task InstallModule(SourcedPackageIdentity id, List<PackageDependencyGroup> dependencies,
            ILogger logger)
        {
            logger.LogInformation($"Install module {id}");

            //already exists with equal version. Do nothing.
            if (_options.ModulesConfig.Modules.Any(x => x.Equals(id)))
            {
                logger.LogInformation("Module already installed. Cancel operation.");
                return;
            }

            var installedModule = _options.ModulesConfig.Modules.FirstOrDefault(x => x.IsSameId(id));
            if (installedModule != null)
                logger.LogInformation(
                    $"Module {installedModule} found (different version). Uninstall existing version.");
        }

        public async Task RemoveModule(PackageIdentity id)
        {
        }
    }

    public interface IModuleLoader
    {
        IImmutableList<PackageIdentity> LoadedModules { get; }
        bool IsLoaded(PackageIdentity module);
    }

    public class ModuleLoader : IModuleLoader
    {
        public IImmutableList<PackageIdentity> LoadedModules => throw new NotImplementedException();

        public bool IsLoaded(PackageIdentity module)
        {
            return false;
        }
    }
}