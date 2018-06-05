using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using Orcus.ModuleManagement;
using Orcus.ModuleManagement.Loader;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Extensions;

namespace Orcus.Server.Service.Modules.Loader
{
    public class ModuleLoader
    {
        private readonly IModulesDirectory _modulesDirectory;

        public ModuleLoader(IModulesDirectory modulesDirectory)
        {
            _modulesDirectory = modulesDirectory;
            ModuleTypeMap = new ModuleTypeMap();
        }

        public ModuleTypeMap ModuleTypeMap { get; }

        public async Task Load(IEnumerable<PackageIdentity> primaryPackages, PackagesLock packagesLock)
        {
            var mapper = new ModuleMapper(FrameworkConstants.CommonFrameworks.OrcusServer10, _modulesDirectory);
            var map = mapper.BuildMap(primaryPackages, packagesLock);

            var loadedAssemblies = new ConcurrentBag<Assembly>();
            while (map.TryPop(out var packages))
            {
                await TaskCombinators.ThrottledAsync(packages, (context, token) => Task.Run(() =>
                {
                    foreach (var file in Directory.GetFiles(context.LibraryDirectory, "*.dll"))
                    {
                        var assemblyName = AssemblyLoadContext.GetAssemblyName(file);
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);

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
