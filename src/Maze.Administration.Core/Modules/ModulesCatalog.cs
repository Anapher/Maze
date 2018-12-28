using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using Maze.ModuleManagement;
using Maze.ModuleManagement.Loader;
using Maze.Server.Connection.Modules;
using Maze.Utilities;

namespace Maze.Administration.Core.Modules
{
    public interface IModulesCatalog
    {
        IImmutableList<PackageCarrier> Packages { get; }
        Task<IReadOnlyList<PackageCarrier>> Load(PackagesLock packagesLock);
    }

    public class ModulesCatalog : IModulesCatalog
    {
        private readonly IModulesDirectory _modulesDirectory;
        private readonly NuGetFramework _framework;

        public ModulesCatalog(IModulesDirectory modulesDirectory, NuGetFramework framework)
        {
            _modulesDirectory = modulesDirectory;
            _framework = framework;
        }

        public IImmutableList<PackageCarrier> Packages { get; private set; } = ImmutableList<PackageCarrier>.Empty;

        public async Task<IReadOnlyList<PackageCarrier>> Load(PackagesLock packagesLock)
        {
            var mapper = new ModuleMapper(_framework, _modulesDirectory, Runtime.Windows, Environment.Is64BitProcess ? Architecture.x64 : Architecture.x86);
            var packageStack = mapper.BuildMap(packagesLock);

            var loadedPackages = new ConcurrentBag<PackageCarrier>();
            while (packageStack.Any())
            {
                var dependencyLayer = packageStack.Pop();

                await TaskCombinators.ThrottledAsync(dependencyLayer, (context, token) =>
                {
                    return Task.Run(() =>
                    {
                        foreach (var module in LoadModule(context))
                        {
                            loadedPackages.Add(module);
                        }
                    });
                }, CancellationToken.None);
            }

            Packages = Packages.AddRange(loadedPackages);

            return loadedPackages.ToList();
        }

        private IEnumerable<PackageCarrier> LoadModule(PackageLoadingContext loadingContext)
        {
            var libFolder = new DirectoryInfo(loadingContext.LibraryDirectory);
            var files = LibraryLocator.GetFilesToLoad(libFolder, loadingContext);

            return files.Select(x => new PackageCarrier(Assembly.LoadFile(x.FullName), loadingContext));
        }
    }
}
