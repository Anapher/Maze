using System.Linq;
using System.Reflection;
using NuGet.Packaging.Core;
using Maze.Modules.Api;

namespace Maze.Server.Service.Modules.Loader
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

            var controllers = types.Where(x => x.IsSubclassOf(typeof(MazeController))).ToList();
            typeMap.Controllers.TryAdd(_packageIdentity, controllers);

            typeMap.Assemblies.Add(_assembly);
        }
    }
}