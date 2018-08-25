using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orcus.Server.ControllersBase;
using Orcus.Server.Options;
using Orcus.Server.Service.Modules;
using Orcus.Server.Service.Modules.Config;
using Orcus.Server.Service.Modules.Loader;
using Orcus.Service.Commander;

namespace Orcus.Server.AppStart
{
    public class PackageInitialization
    {
        private readonly ModulesOptions _options;
        private readonly IServiceCollection _serviceCollection;

        public PackageInitialization(ModulesOptions options, IServiceCollection serviceCollection)
        {
            _options = options;
            _serviceCollection = serviceCollection;
        }

        private void InvalidateModulesLock()
        {
            var modulesConfigFile = new FileInfo(_options.ModulesFile);
            if (!modulesConfigFile.Exists)
                return;

            var modulesLockFile = new FileInfo(_options.ModulesLock);
            if (!modulesLockFile.Exists)
                return;

            if (modulesConfigFile.LastWriteTimeUtc > modulesLockFile.LastWriteTimeUtc)
                modulesLockFile.Delete(); //invalidate
        }

        public async Task LoadModules(ContainerBuilder builder)
        {
            InvalidateModulesLock();

            var modulesConfig = new ModulesConfig(_options.ModulesFile);
            var modulesLock = new ModulesLock(_options.ModulesLock);

            await modulesConfig.Reload();
            await modulesLock.Reload();

            var orcusProject = new OrcusProject(_options, modulesConfig, modulesLock);
            _serviceCollection.AddSingleton<IModuleProject>(orcusProject);
            _serviceCollection.AddSingleton<PackageLockRequestManager>();
            _serviceCollection.AddSingleton<IModulePackageManager, ModulePackageManager>();

            var provider = _serviceCollection.BuildServiceProvider();
            var logger = provider.GetService<ILogger<PackageInitialization>>();
            var packageManager = provider.GetService<IModulePackageManager>();

            logger.LogInformation("Initialize modules");

            //initialize all core frameworks
            foreach (var framework in orcusProject.FrameworkLibraries.Keys.Where(x => x != orcusProject.Framework))
            {
                logger.LogInformation("Initialize modules lock for {framework}", framework);
                var packages = await packageManager.GetPackagesLock(framework);
                await packageManager.EnsurePackagesInstalled(packages);
            }

            logger.LogInformation("Initialize modules lock for {framework}", orcusProject.Framework);
            var packageLock = await packageManager.GetPackagesLock(orcusProject.Framework);
            await packageManager.EnsurePackagesInstalled(packageLock);

            if (modulesConfig.Modules.Any())
            {
                logger.LogDebug("{count} modules found", modulesConfig.Modules.Count);

                var loader = new ModuleLoader(orcusProject, AssemblyLoadContext.Default);
                await loader.Load(modulesConfig.Modules, packageLock);

                loader.ModuleTypeMap.Configure(builder);

                builder.RegisterInstance(new ModuleControllerProvider(loader.ModuleTypeMap)).AsImplementedInterfaces();
                builder.RegisterOrcusServices(cache => cache.BuildCache(loader.ModuleTypeMap.Controllers));
            }
            else
            {
                logger.LogDebug("No modules found, startup empty");
                builder.RegisterInstance(new ModuleControllerProvider()).AsImplementedInterfaces();
                builder.RegisterOrcusServices(cache => cache.BuildEmpty());
            }

            builder.RegisterInstance(modulesConfig).AsImplementedInterfaces();
            builder.RegisterInstance(modulesLock).AsImplementedInterfaces();
        }
    }
}