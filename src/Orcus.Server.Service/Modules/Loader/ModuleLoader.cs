﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using Orcus.ModuleManagement.Loader;
using Orcus.Modules.Api.Utilities;
using Orcus.Server.Connection.Modules;

namespace Orcus.Server.Service.Modules.Loader
{
    public class ModuleLoader
    {
        private readonly IModuleProject _project;
        private readonly AssemblyLoadContext _assemblyLoadContext;
        private IReadOnlyDictionary<AssemblyName, AssemblyInfo> _dependencyAssemblies;

        public ModuleLoader(IModuleProject project, AssemblyLoadContext assemblyLoadContext)
        {
            _project = project;
            _assemblyLoadContext = assemblyLoadContext;
            _assemblyLoadContext.Resolving += DefaultOnResolving;

            ModuleTypeMap = new ModuleTypeMap();
        }

        public ModuleTypeMap ModuleTypeMap { get; }

        public async Task Load(IEnumerable<PackageIdentity> primaryPackages, PackagesLock packagesLock)
        {
            var mapper = new ModuleMapper(_project.Framework, _project.ModulesDirectory, _project.Runtime, _project.Architecture);
            var map = mapper.BuildMap(packagesLock);
            
            var dependencyPaths = new Dictionary<AssemblyName, AssemblyInfo>();
            _dependencyAssemblies = dependencyPaths;

            while (map.TryPop(out var dependencyLayer))
            {
                foreach (var context in dependencyLayer.Where(x => !x.IsOrcusModule))
                {
                    foreach (var file in Directory.GetFiles(context.LibraryDirectory, "*.dll"))
                    {
                        dependencyPaths.Add(AssemblyName.GetAssemblyName(file), new AssemblyInfo(file));
                    }
                }

                await TaskCombinators.ThrottledAsync(dependencyLayer.Where(x => x.IsOrcusModule), (context, token) => Task.Run(() =>
                {
                    foreach (var file in Directory.GetFiles(context.LibraryDirectory, "*.dll"))
                    {
                        var assembly = _assemblyLoadContext.LoadFromAssemblyPath(file);

                        var typeDiscoveryService = new TypeDiscoveryService(assembly, context.Package);
                        typeDiscoveryService.DiscoverTypes(ModuleTypeMap);
                    }
                }), CancellationToken.None);
            }
        }
        
        private Assembly DefaultOnResolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            if (_dependencyAssemblies.TryGetValue(arg2, out var assemblyInfo))
                return assemblyInfo.LoadAssembly(arg1);

            return null;
        }
    }
}
