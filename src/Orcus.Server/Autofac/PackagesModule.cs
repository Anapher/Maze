using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Autofac;
using Orcus.Server.ControllersBase;
using Orcus.Server.Options;
using Orcus.Server.Service.Modules;
using Orcus.Server.Service.Modules.Config;
using Orcus.Server.Service.Modules.Loader;
using Orcus.Service.Commander;

namespace Orcus.Server.Autofac
{
    public class PackagesModule : Module
    {
        private readonly ModulesOptions _options;

        public PackagesModule(ModulesOptions options)
        {
            _options = options;
        }

        protected override void Load(ContainerBuilder builder)
        {
            AsyncLoad(builder).Wait();
        }

        protected async Task AsyncLoad(ContainerBuilder builder)
        {
            var modulesConfig = new ModulesConfig(_options.ModulesFile);
            var modulesLock = new ModulesLock(_options.ModulesLock);

            await modulesConfig.Reload();
            await modulesLock.Reload();

            var orcusProject = new OrcusProject(_options, modulesConfig, modulesLock);
            if (modulesConfig.Modules.Any())
            {
                var loader = new ModuleLoader(orcusProject, AssemblyLoadContext.Default);
                await loader.Load(modulesConfig.Modules, modulesLock.Modules[orcusProject.Framework]);

                loader.ModuleTypeMap.Configure(builder);

                builder.RegisterInstance(new ModuleControllerProvider(loader.ModuleTypeMap)).AsImplementedInterfaces();
                builder.RegisterOrcusServices(cache => cache.BuildCache(loader.ModuleTypeMap.Controllers));
            }
            else
            {
                builder.RegisterInstance(new ModuleControllerProvider()).AsImplementedInterfaces();
                builder.RegisterOrcusServices(cache => cache.BuildEmpty());
            }

            builder.RegisterInstance(modulesConfig).AsImplementedInterfaces();
            builder.RegisterInstance(modulesLock).AsImplementedInterfaces();
            builder.RegisterInstance(orcusProject).AsImplementedInterfaces();
        }
    }
}