using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Frameworks;
using NuGet.Packaging.Core;

namespace Orcus.ModuleManagement.Loader
{
    public class ModuleMapper
    {
        public ModuleMapper(NuGetFramework orcusFramework, IModulesDirectory modulesDirectory)
        {
            OrcusFramework = orcusFramework;
            ModulesDirectory = modulesDirectory;
        }

        public NuGetFramework OrcusFramework { get; }
        public IModulesDirectory ModulesDirectory { get; }

        /// <summary>
        ///     Build a module loader map that determines in which order which packages must be loaded
        /// </summary>
        /// <param name="primaryPackages">The primary packages</param>
        /// <param name="packagesLock">The packages lock</param>
        /// <returns>Return a stack of dependency lists that can be loaded in parallel</returns>
        public Stack<List<PackageLoadingContext>> BuildMap(IEnumerable<PackageIdentity> primaryPackages,
            PackagesLock packagesLock)
        {
            var levelMap = new Dictionary<PackageIdentity, int>(PackageIdentity.Comparer);

            foreach (var packageIdentity in primaryPackages)
                SearchDependencies(packageIdentity, packagesLock, levelMap, 0);

            var result = new Stack<List<PackageLoadingContext>>();

            //the first item of this foreach loop must be loaded last (because it has the lowest level)
            foreach (var levelGroup in levelMap.GroupBy(x => x.Value).OrderBy(x => x.Key))
            {
                var levelList = new List<PackageLoadingContext>();

                foreach (var packageIdentity in levelGroup.Select(x => x.Key))
                {
                    if (!ModulesDirectory.ModuleExists(packageIdentity))
                        throw new FileNotFoundException($"The package {packageIdentity} was not found.");

                    var packageDirectory =
                        ModulesDirectory.VersionFolderPathResolver.GetPackageDirectory(packageIdentity.Id,
                            packageIdentity.Version);
                    var (libDirectory, framework) = GetFrameworkDirectory(packageDirectory);

                    levelList.Add(new PackageLoadingContext(packageIdentity, packageDirectory, libDirectory.FullName,
                        framework, framework.Framework == OrcusFramework.Framework));
                }

                result.Push(levelList);
            }

            return result;
        }

        private (DirectoryInfo folder, NuGetFramework framework) GetFrameworkDirectory(string packageDirectory)
        {
            var libFolder = new DirectoryInfo(Path.Combine(packageDirectory, "lib"));
            if (!libFolder.Exists)
                return (null, null);

            var frameworkFolders = libFolder.GetDirectories()
                .ToDictionary(x => NuGetFramework.ParseFolder(x.Name), x => x, NuGetFramework.Comparer);
            //if (frameworkFolders.TryGetValue(OrcusFramework, out var directory))
            //    return (directory, OrcusFramework);

            var bestFramework =
                NuGetFrameworkUtility.GetNearest(frameworkFolders.Select(x => x.Key), OrcusFramework, x => x);
            if (bestFramework == null)
                return (null, null);

            return (frameworkFolders[bestFramework], bestFramework);
        }

        private static void SearchDependencies(PackageIdentity packageIdentity, PackagesLock packagesLock,
            IDictionary<PackageIdentity, int> map, int level)
        {
            if (!map.TryGetValue(packageIdentity, out var currentLevel) || level > currentLevel)
                map[packageIdentity] = level;

            foreach (var dependency in packagesLock.Packages[packageIdentity])
                SearchDependencies(dependency, packagesLock, map, level + 1);
        }
    }
}