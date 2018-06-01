using System.Linq;
using System.Reflection;
using NuGet.Packaging.Core;
using Orcus.Modules.Api;

namespace Orcus.Server.Service.Modules.Loader
{
    public class TypeDiscoveryService
    {
        private readonly Assembly _assembly;
        private readonly PackageIdentity _packageIdentity;

        public TypeDiscoveryService(Assembly assembly, PackageIdentity packageIdentity)
        {
            _assembly = assembly;
            _packageIdentity = packageIdentity;
        }

        public void DiscoverTypes(ModuleTypeMap typeMap)
        {
            var types = _assembly.GetExportedTypes();

            var controllers = types.Where(x => x.IsSubclassOf(typeof(OrcusController))).ToList();
            typeMap.Controllers.TryAdd(_packageIdentity, controllers);

            var startupTypes = types.Where(x => x.Name == "Startup").ToList();
            if (startupTypes.Count > 1)
                throw new ModuleLoadingException(
                    "More than one startup class found. Please make sure that only one public class called 'Startup' exists.",
                    _assembly);

            if (startupTypes.Any())
                typeMap.Startup.Add(startupTypes.Single());
        }
    }
}