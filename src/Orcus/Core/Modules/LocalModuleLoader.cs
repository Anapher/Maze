using System.Linq;
using Autofac;
using Microsoft.Extensions.Options;
using Orcus.Options;

namespace Orcus.Core.Modules
{
    public interface ILocalModuleLoader
    {
        bool Load(ContainerBuilder builder);
    }

    public class LocalModuleLoader : ILocalModuleLoader
    {
        private readonly IModulesCatalog _catalog;
        private readonly IPackagesRegistrar _packagesRegistrar;
        private readonly ModulesOptions _options;

        public LocalModuleLoader(IOptions<ModulesOptions> options, IModulesCatalog catalog,
            IPackagesRegistrar packagesRegistrar)
        {
            _catalog = catalog;
            _packagesRegistrar = packagesRegistrar;
            _options = options.Value;
        }

        public bool Load(ContainerBuilder builder)
        {
            var primaryModules = _options.GetPrimaryModules();
            if (primaryModules?.Any() != true)
                return false;

            var packages = _catalog.Load(primaryModules, _options.GetPackagesLock());
            _packagesRegistrar.Configure(builder, packages);
            return true;
        }
    }
}