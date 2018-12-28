using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NuGet.Packaging.Core;
using Maze.Modules.Api;
using Maze.Server.Connection.Modules;
using Maze.Utilities;

namespace Maze.Core.Modules
{
    public interface IPackageLockLoader
    {
        Task<PackageLockContext> Load(PackagesLock packagesLock);
    }

    public class PackageLockContext
    {
        private readonly IModulesCatalog _catalog;
        private readonly IReadOnlyList<PackageCarrier> _packages;
        private readonly IPackagesRegistrar _registrar;

        public PackageLockContext(IModulesCatalog catalog)
        {
            _catalog = catalog;
            PackagesLoaded = false;
        }

        public PackageLockContext(IReadOnlyList<PackageCarrier> packages, IPackagesRegistrar registrar,
            IModulesCatalog catalog)
        {
            _packages = packages;
            _registrar = registrar;
            _catalog = catalog;

            PackagesLoaded = true;
        }

        public bool PackagesLoaded { get; }

        public void Configure(ContainerBuilder builder)
        {
            if (!PackagesLoaded)
                throw new NotSupportedException("The context was not initialized with success");

            _registrar.Configure(builder, _packages);
        }

        public async Task<IReadOnlyDictionary<PackageIdentity, List<Type>>> GetControllers()
        {
            var result = new ConcurrentDictionary<PackageIdentity, List<Type>>();
            var mazeControllerType = typeof(MazeController);

            await TaskCombinators.ThrottledAsync(_catalog.Packages, (carrier, token) => Task.Run(() =>
            {
                var types = carrier.Assembly.GetExportedTypes();

                var controllers = types.Where(x => mazeControllerType.IsAssignableFrom(x)).ToList();
                result.AddOrUpdate(carrier.Context.Package, controllers, (identity, list) =>
                {
                    list.AddRange(controllers);
                    return list;
                });
            }), CancellationToken.None);

            return result;
        }
    }

    public class PackageLockLoader : IPackageLockLoader
    {
        private readonly IModulesCatalog _catalog;
        private readonly IPackagesRegistrar _packagesRegistrar;

        public PackageLockLoader(IModulesCatalog catalog, IPackagesRegistrar packagesRegistrar)
        {
            _catalog = catalog;
            _packagesRegistrar = packagesRegistrar;
        }

        public async Task<PackageLockContext> Load(PackagesLock packagesLock)
        {
            if (packagesLock?.Any() != true)
                return new PackageLockContext(_catalog);

            var packages = await _catalog.Load(packagesLock);
            return new PackageLockContext(packages, _packagesRegistrar, _catalog);
        }
    }
}