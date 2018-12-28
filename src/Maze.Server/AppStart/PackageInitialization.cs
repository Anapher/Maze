using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Maze.Server.ControllersBase;
using Maze.Server.Options;
using Maze.Server.Service.Modules;
using Maze.Server.Service.Modules.Config;
using Maze.Server.Service.Modules.Loader;
using Maze.Service.Commander;

namespace Maze.Server.AppStart
{
    public class PackageInitialization
    {
        private readonly ModulesOptions _options;
        private readonly IServiceCollection _serviceCollection;
        private readonly IConfiguration _configuration;

        public PackageInitialization(ModulesOptions options, IServiceCollection serviceCollection, IConfiguration configuration)
        {
            _options = options;
            _serviceCollection = serviceCollection;
            _configuration = configuration;
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

        public async Task LoadModules(ContainerBuilder builder, IMvcBuilder mvcBuilder)
        {
            InvalidateModulesLock();

            var modulesConfig = new ModulesConfig(_options.ModulesFile);
            var modulesLock = new ModulesLock(_options.ModulesLock);

            await modulesConfig.Reload();
            await modulesLock.Reload();

            var mazeProject = new MazeProject(_options, modulesConfig, modulesLock);
            _serviceCollection.AddSingleton<IModuleProject>(mazeProject);
            _serviceCollection.AddSingleton<PackageLockRequestManager>();
            _serviceCollection.AddSingleton<IModulePackageManager, ModulePackageManager>();
            _serviceCollection.AddSingleton<IVirtualModuleManager, VirtualModuleManager>();

            var provider = _serviceCollection.BuildServiceProvider();
            var logger = provider.GetService<ILogger<PackageInitialization>>();
            var packageManager = provider.GetService<IModulePackageManager>();

            logger.LogInformation("Initialize modules");

            //initialize all core frameworks
            foreach (var framework in mazeProject.FrameworkLibraries.Keys.Where(x => x != mazeProject.Framework))
            {
                logger.LogInformation("Initialize modules lock for {framework}", framework);
                var packages = await packageManager.GetPackagesLock(framework);
                await packageManager.EnsurePackagesInstalled(packages);
            }

            logger.LogInformation("Initialize modules lock for {framework}", mazeProject.Framework);
            var packageLock = await packageManager.GetPackagesLock(mazeProject.Framework);
            await packageManager.EnsurePackagesInstalled(packageLock);

            if (modulesConfig.Modules.Any())
            {
                logger.LogDebug("{count} modules found", modulesConfig.Modules.Count);

                var loader = new ModuleLoader(mazeProject, _configuration, AssemblyLoadContext.Default);
                await loader.Load(modulesConfig.Modules, packageLock);

                loader.ModuleTypeMap.Configure(builder);

                foreach (var assembly in loader.ModuleTypeMap.Assemblies)
                    mvcBuilder.AddApplicationPart(assembly);
                
                builder.RegisterInstance(new ModuleControllerProvider(loader.ModuleTypeMap)).AsImplementedInterfaces();
                builder.RegisterMazeServices(cache => cache.BuildCache(loader.ModuleTypeMap.Controllers));
            }
            else
            {
                logger.LogDebug("No modules found, startup empty");
                builder.RegisterInstance(new ModuleControllerProvider()).AsImplementedInterfaces();
                builder.RegisterMazeServices(cache => cache.BuildEmpty());
            }

            builder.RegisterInstance(modulesConfig).AsImplementedInterfaces();
            builder.RegisterInstance(modulesLock).AsImplementedInterfaces();
        }
    }
}