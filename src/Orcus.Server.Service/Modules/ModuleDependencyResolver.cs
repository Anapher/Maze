using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.Config;

namespace Orcus.Server.Service.Modules
{
    public class ModuleDependencyResolver
    {
        private readonly IModuleManager _moduleManager;

        public ModuleDependencyResolver(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        //public async Task<bool> ResolveDependencies(IEnumerable<PackageDependency> dependencies, ILogger logger)
        //{
        //    foreach (var packageDependency in dependencies)
        //    {
        //        var loadedModule = _moduleManager.LoadedModules.FirstOrDefault(x => string.Equals(x.Id.Id, packageDependency.Id, StringComparison.OrdinalIgnoreCase));
        //        if (loadedModule != null)
        //        {
        //            if (packageDependency.VersionRange.Satisfies(loadedModule.Id.Version))
        //                continue; //there is already a module loaded that has a correct version

        //            logger.LogError(
        //                $"The dependency '{packageDependency.Id}' could not be loaded because this dependency is already loaded with a version ({loadedModule.Id.Version}) that does not satisfy the required version ({packageDependency.VersionRange})");
        //            return false;
        //        }
                
        //    }
        //}
    }

    public interface IModuleManager
    {
        IModulesConfig ModulesConfig { get; }
        IRepositorySourceConfig RepositorySourcesConfig { get; }
        IImmutableList<ModuleInfo> LoadedModules { get; }
        IList<IGrouping<string, (PackageIdentity identity, string path)>> AvailableModules { get; }

        void LoadModule(string path, ILogger logger);
        Task InstallModule(SourcedPackageIdentity packageIdentity);
    }
}