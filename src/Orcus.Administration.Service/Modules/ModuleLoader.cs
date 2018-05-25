using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using Orcus.Administration.Core.Clients;
using Orcus.ModuleManagement;
using Orcus.Server.Connection.Modules;

namespace Orcus.Administration.Service.Modules
{
    public class ModuleLoader
    {
        public ModuleLoader(string modulesPath)
        {
            ModulesDirectory = new ModulesDirectory(modulesPath);
        }

        public IModulesDirectory ModulesDirectory { get; }

        public async Task LoadModules(ModuleMap modules, IOrcusRestClient client)
        {
            var requiredModules = modules.PackageDependencies.Select(x => x.Key).ToList();
            var nonExistingModules = new List<PackageIdentity>();

            foreach (var packageIdentity in requiredModules)
            {
                if (!ModulesDirectory.ModuleExists(packageIdentity))
                    nonExistingModules.Add(packageIdentity);
            }

            if (nonExistingModules.Any())
            {
                Taskco
            }
        }
    }
}