using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Orcus.ModuleManagement;
using Orcus.ModuleManagement.Loader;
using Orcus.Server.Connection.Modules;
using Orcus.Utilities;

namespace Orcus.Core.Modules
{
    public interface IModulesCatalog
    {
        IImmutableList<PackageCarrier> Packages { get; }
        Task<IReadOnlyList<PackageCarrier>> Load(PackagesLock packagesLock);
    }

    public class ModulesCatalog : IModulesCatalog
    {
        private readonly IApplicationInfo _applicationInfo;
        private readonly IPackageLoader _packageLoader;
        private readonly IModulesDirectory _modulesDirectory;

        public ModulesCatalog(IModulesDirectory modulesDirectory, IApplicationInfo applicationInfo, IPackageLoader packageLoader)
        {
            _modulesDirectory = modulesDirectory;
            _applicationInfo = applicationInfo;
            _packageLoader = packageLoader;
        }

        public IImmutableList<PackageCarrier> Packages { get; private set; } = ImmutableList<PackageCarrier>.Empty;

        public async Task<IReadOnlyList<PackageCarrier>> Load(PackagesLock packagesLock)
        {
            var mapper = new ModuleMapper(_applicationInfo.Framework, _modulesDirectory, Runtime.Windows, Environment.Is64BitProcess ? Architecture.x64 : Architecture.x86);
            var packageStack = mapper.BuildMap(packagesLock);

            var loadedPackages = new ConcurrentBag<PackageCarrier>();
            while (packageStack.Any())
            {
                var dependencyLayer = packageStack.Pop();

                await TaskCombinators.ThrottledAsync(dependencyLayer, (context, token) =>
                {
                    if (Packages.Any(x => x.Context.PackageDirectory == context.PackageDirectory))
                        return Task.CompletedTask;

                    return Task.Run(() =>
                    {
                        foreach (var module in _packageLoader.Load(context))
                        {
                            loadedPackages.Add(module);
                        }
                    });
                }, CancellationToken.None);
            }

            Packages = Packages.AddRange(loadedPackages);

            return loadedPackages.ToList();
        }
    }
}