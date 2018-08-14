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
using Orcus.ModuleManagement;
using Orcus.ModuleManagement.Loader;
using Orcus.ModuleManagement.Utilities;
using Orcus.Server.Connection.Modules;

namespace Orcus.Administration.Core.Modules
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
                    return Task.Run(() => loadedPackages.Add(LoadModule(context)));
                }, CancellationToken.None);
            }

            Packages = Packages.AddRange(loadedPackages);

            return loadedPackages.ToList();
        }

        private PackageCarrier LoadModule(PackageLoadingContext loadingContext)
        {
            var libFolder = new DirectoryInfo(loadingContext.LibraryDirectory);
            var dlls = libFolder.GetFiles("*.dll");

            if (!dlls.Any())
                throw new InvalidOperationException(
                    $"Cannot load package {loadingContext.Package} ({loadingContext.PackageDirectory}) because there are no dlls");

            if (dlls.Length > 1)
                throw new NotImplementedException();

            var file = dlls.Single();
            return new PackageCarrier(Assembly.LoadFile(file.FullName), loadingContext);
        }
    }
}
