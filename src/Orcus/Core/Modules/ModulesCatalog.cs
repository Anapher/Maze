using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NuGet.Packaging.Core;
using Orcus.ModuleManagement;
using Orcus.ModuleManagement.Loader;
using Orcus.Server.Connection.Modules;

namespace Orcus.Core.Modules
{
    public interface IModulesCatalog
    {
        IImmutableList<PackageCarrier> Packages { get; }
        IReadOnlyList<PackageCarrier> Load(IReadOnlyList<PackageIdentity> packages, PackagesLock packagesLock);
    }

    public class ModulesCatalog : IModulesCatalog
    {
        private readonly IApplicationInfo _applicationInfo;
        private readonly IPackageLoader _packageLoader;
        private readonly IModulesDirectory _modulesDirectory;
        private bool _isLoaded;

        public ModulesCatalog(IModulesDirectory modulesDirectory, IApplicationInfo applicationInfo, IPackageLoader packageLoader)
        {
            _modulesDirectory = modulesDirectory;
            _applicationInfo = applicationInfo;
            _packageLoader = packageLoader;
        }

        public IImmutableList<PackageCarrier> Packages { get; private set; } = new ImmutableArray<PackageCarrier>();

        public IReadOnlyList<PackageCarrier> Load(IReadOnlyList<PackageIdentity> packages, PackagesLock packagesLock)
        {
            if (_isLoaded)
                throw new InvalidOperationException("Cannot load packages twice");
            _isLoaded = true;

            var mapper = new ModuleMapper(_applicationInfo.Framework, _modulesDirectory);
            var packageStack = mapper.BuildMap(packages, packagesLock);

            var loadedPackages = new List<PackageCarrier>();

            while (packageStack.Any())
            {
                var dependencyLayer = packageStack.Pop();
                foreach (var package in dependencyLayer)
                {
                    if (Packages.Any(x => x.Context.PackageDirectory == package.PackageDirectory))
                        continue;

                    loadedPackages.Add(_packageLoader.Load(package));
                }
            }

            Packages = Packages.AddRange(loadedPackages);

            return loadedPackages;
        }
    }
}