using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.ModulesV1.Config;

namespace Orcus.Server.Service.ModulesV1
{
    //https://github.com/NuGet/Home/issues/5674
    public interface IModuleManager
    {
        IModulesConfig ModulesConfig { get; }
        IRepositorySourceConfig RepositorySourcesConfig { get; }
        IImmutableList<ModuleInfo> LoadedModules { get; }
        //IList<IGrouping<string, (PackageIdentity identity, string path)>> AvailableModules { get; }
        Task InstallModule(PackageIdentity identity, ILogger logger);
        Task InstallModule(SourcedPackageIdentity identity, ILogger logger);

        void LoadModule(string path, ILogger logger);
    }

    public class ModuleManager : IModuleManager
    {
        private readonly string _modulesDirectory;
        private readonly string _cacheDirectory;
        private readonly string _configDirectory;
        private readonly object _modulesLock = new object();

        public ModuleManager(string modulesDir, string cacheDir, string configDir, IModulesConfig modulesConfig,
            IRepositorySourceConfig repositorySourcesConfig)
        {
            _modulesDirectory = modulesDir;
            Directory.CreateDirectory(_modulesDirectory);
            _cacheDirectory = cacheDir;
            Directory.CreateDirectory(_cacheDirectory);
            _configDirectory = configDir;
            Directory.CreateDirectory(_configDirectory);

            ModulesConfig = modulesConfig;
            RepositorySourcesConfig = repositorySourcesConfig;
        }

        public IImmutableList<ModuleInfo> LoadedModules { get; private set; } = new List<ModuleInfo>().ToImmutableList();
        public IModulesConfig ModulesConfig { get; }
        public IRepositorySourceConfig RepositorySourcesConfig { get; }

        public async Task LoadModules(ILogger logger)
        {
            try
            {
                await ModulesConfig.Reload();
            }
            catch (Exception e)
            {
                logger.LogError($"Something went wrong when trying to read the file {Path.GetFileName(ModulesConfig.Path)}: {e.Message}");
            }

            var moduleFiles = new DirectoryInfo(_modulesDirectory).GetFiles("*", SearchOption.TopDirectoryOnly)
                .ToDictionary(x => GetModuleFileIdentity(x.FullName), x => x).GroupBy(x => x.Key.Id).ToList();

            foreach (var module in ModulesConfig.Items)
            {
                var versions = moduleFiles.FirstOrDefault(x => x.Key == module.Id);
                if (versions == null)
                {
                    logger.LogInformation($"The module {module.Id} was not found. Installing module...");
                    await InstallModule(module, logger);
                    continue;
                }

                var newestAvailableVersion = versions.OrderByDescending(x => x.Key.Version).First();
                if (newestAvailableVersion.Key.Version < module.Version)
                {
                    logger.LogWarning(
                        $"The module {module.Id} version {module.Version} was not found (newest version found: {newestAvailableVersion.Key.Version}). Download newer version.");
                    await InstallModule(module, logger);
                }
                else
                {
                    if (newestAvailableVersion.Key.Version > module.Version)
                        logger.LogWarning(
                            $"A newer version of module {module.Id} was found. Version of config: {module.Version}; version found: {newestAvailableVersion.Key.Version}. Use newest version.");
                    LoadModule(newestAvailableVersion.Value.FullName, logger);
                }
            }

            var unknownModules = moduleFiles.Where(x => LoadedModules.All(y => y.Id.Id != x.Key));
            foreach (var unknownModule in unknownModules)
            {
                logger.LogWarning(
                    $"The module {unknownModule.Key} was not specified in the {Path.GetFileName(ModulesConfig.Path)} file. The module will be loaded anyways.");
                LoadModule(unknownModule.OrderByDescending(x => x.Key.Version).First().Value.FullName, logger);
            }

            logger.LogInformation($"{LoadedModules.Count} modules were loaded.");
        }

        public Task InstallModule(PackageIdentity identity, ILogger logger) =>
            InstallModule(new SourcedPackageIdentity(identity.Id, identity.Version, OfficalOrcusRepository.Uri), logger);

        public async Task InstallModule(SourcedPackageIdentity identity, ILogger logger)
        {
            SourceRepository sourceRepository;

            if (identity.SourceRepository == null || identity.SourceRepository == OfficalOrcusRepository.Uri)
                sourceRepository = RepositorySourcesConfig.OfficalRepository;
            else
            {
                sourceRepository =
                    RepositorySourcesConfig.SourceRepositories.FirstOrDefault(x =>
                        x.PackageSource.SourceUri == identity.SourceRepository);
                if (sourceRepository == null)
                    throw new ArgumentException(
                        $"The source repository with uri {identity.SourceRepository} was not found.", nameof(identity));
            }

            var moduleFile = await sourceRepository.DirectDownloadAsync(identity, _modulesDirectory,
                logger, CancellationToken.None);
            
            await ModulesConfig.AddItem(identity);

            LoadModule(moduleFile.FullName, logger);
        }

        private void AddModule(ModuleInfo module)
        {
            lock (_modulesLock)
            {
                LoadedModules = LoadedModules.Add(module);
            }
        }

        public PackageIdentity GetModuleFileIdentity(string path)
        {
            using (var moduleFs = File.OpenRead(path))
            using (var reader = new PackageArchiveReader(moduleFs))
                return reader.NuspecReader.GetIdentity();
        }

        //https://github.com/ExtCore/ExtCore/blob/f57a387410426f41d365ad17797294a110c63d52/src/ExtCore.WebApplication/DefaultAssemblyProvider.cs
        public void LoadModule(string path, ILogger logger)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("The file path was not found.");

            using (var moduleFs = File.OpenRead(path))
            using (var reader = new PackageArchiveReader(moduleFs))
            {
                var identity = reader.NuspecReader.GetIdentity();
                var dependencies = reader.GetPackageDependencies().ToList();
                var serverDependencies = dependencies.FirstOrDefault(x => x.TargetFramework == OrcusFrameworks.Server);

                var references = reader.GetReferenceItems().ToList();

                var moduleDto = new InstalledModule
                {
                    Id = identity,
                    Title = reader.NuspecReader.GetTitle(),
                    Authors = reader.NuspecReader.GetAuthors(),
                    Owners = reader.NuspecReader.GetOwners(),
                    LicenseUrl = reader.NuspecReader.GetLicenseUrl(),
                    ProjectUrl = reader.NuspecReader.GetProjectUrl(),
                    IconUrl = reader.NuspecReader.GetIconUrl(),
                    Description = reader.NuspecReader.GetDescription(),
                    Summary = reader.NuspecReader.GetSummary(),
                    Copyright = reader.NuspecReader.GetCopyright(),

                    Dependencies = dependencies,
                    IsAdministrationLoadable = references.Any(x => x.TargetFramework.Framework == "uap"),
                    IsServerLoadable = references.Any(x => x.TargetFramework.Framework == "net"),
                    IsClientLoadable = references.Any(x => x.TargetFramework.Framework == "win"),
                };

                var moduleInfo = new ModuleInfo {Id = identity, Dto = moduleDto, Filename = path, IsLoaded = false};
                AddModule(moduleInfo);

                var serverReferences = references.FirstOrDefault(x => x.TargetFramework.Framework == "net");
                if (serverReferences == null)
                {
                    logger.LogInformation($"The package {identity} doesn't contain module files for servers.");
                    return;
                }

                var cacheDirectory = new DirectoryInfo(Path.Combine(_cacheDirectory, identity.GetFilename()));
                if (!cacheDirectory.Exists)
                {
                    var tempDirectory =
                        new DirectoryInfo(Path.Combine(_cacheDirectory, identity.GetFilename() + "-tmp"));
                    if (tempDirectory.Exists)
                        tempDirectory.Delete(true);

                    tempDirectory.Create();

                    var serverFiles = reader.GetFiles("lib\\net");
                    reader.CopyFiles(tempDirectory.FullName, serverFiles, ExtractFile, logger, CancellationToken.None);

                    tempDirectory.MoveTo(cacheDirectory.FullName);
                }

                cacheDirectory.Refresh();

                foreach (var serverReference in serverReferences.Items)
                {
                    var serverFile = Path.Combine(cacheDirectory.FullName, serverReference);
                    if (!File.Exists(serverFile))
                    {
                        logger.LogError(
                            $"The reference file '{serverReference}' of module '{identity.GetFilename()}' was not found (search path: {serverFile}). The file is skipped, other references of that module will be attempted to load.");
                        continue;
                    }

                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(serverFile);
                    LoadModuleAssembly(assembly);
                }
            }
        }

        private void LoadModuleAssembly(Assembly assembly)
        {

        }

        private string ExtractFile(string sourcefile, string targetpath, Stream filestream)
        {
            using (var destinationFile = File.Create(targetpath))
                filestream.CopyTo(destinationFile);

            return targetpath;
        }
    }
}