namespace Orcus.Server.Service.ModulesV1
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


}