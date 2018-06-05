using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using Orcus.Administration.Core.Clients;
using Orcus.Server.Connection.Modules;

namespace Orcus.Administration.Core.Modules
{
    public class ModuleLoader
    {

        public IModulesDirectory ModulesDirectory { get; set; }

        public async Task LoadModules(ModuleMap modules, IOrcusRestClient client)
        {
            var requiredModules = modules.PackageDependencies.Select(x => x.Key).ToList();
            var nonExistingModules = new List<PackageIdentity>();

            foreach (var packageIdentity in requiredModules)
            {

            }
        }
    }
}