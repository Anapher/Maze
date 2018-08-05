using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using Orcus.ModuleManagement.Loader;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Extensions;

namespace Orcus.Server.Service.Modules.Loader
{
    public class ModuleLoader
    {
        private readonly IModuleProject _project;

        public ModuleLoader(IModuleProject project)
        {
            _project = project;
            ModuleTypeMap = new ModuleTypeMap();
        }

        public ModuleTypeMap ModuleTypeMap { get; }

        public async Task Load(IEnumerable<PackageIdentity> primaryPackages, PackagesLock packagesLock)
        {
            var mapper = new ModuleMapper(_project.Framework, _project.ModulesDirectory, _project.Runtime, _project.Architecture);
            var map = mapper.BuildMap(primaryPackages, packagesLock);

            var loadedAssemblies = new ConcurrentBag<Assembly>();
            while (map.TryPop(out var dependencyLayer))
            {
                await TaskCombinators.ThrottledAsync(dependencyLayer, (context, token) => Task.Run(() =>
                {
                    foreach (var file in Directory.GetFiles(context.LibraryDirectory, "*.dll"))
                    {
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);

                        if (context.IsOrcusModule)
                        {
                            var service = new TypeDiscoveryService(assembly, context.Package);
                            service.DiscoverTypes(ModuleTypeMap);
                        }

                        loadedAssemblies.Add(assembly);
                    }
                }), CancellationToken.None);
            }
        }
    }
}
