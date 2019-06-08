using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Exceptions;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Rest.ClientConfigurations.V1;
using Maze.Administration.Library.Rest.Modules.V1;
using Maze.Client.Administration.Core.Utilities;
using Maze.Client.Administration.Core.Wix;
using Maze.Client.Administration.Core.Wix.Tools;
using Maze.ModuleManagement;
using Maze.ModuleManagement.PackageManagement;
using Maze.ModuleManagement.Server;
using Maze.Server.Connection.Clients;
using Maze.Server.Connection.Modules;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

namespace Maze.Client.Administration.Core
{
    public interface IMazeWixBuilder
    {
        Task Build(IEnumerable<ClientGroupViewModel> groups, string outputFilename, ILogger logger, CancellationToken cancellationToken);
    }

    public class MazeWixBuilder : IMazeWixBuilder
    {
        private const string ReleaseComponentsFilename = "Components.Release.Generated.wxs";
        private const string AppDataComponentsFilename = "Components.AppData.Generated.wxs";
        private const string PackagesComponentsFilename = "Components.Packages.Generated.wxs";

        private readonly string _clientFilesDirectory;
        private readonly IWixToolRunner _wixToolRunner;
        private readonly IMazeRestClient _restClient;
        private readonly IFileSystem _fileSystem;
        private readonly VersionFolderPathResolver _versionFolderPathResolver;

        private readonly IReadOnlyList<string> _wixExtensions = new[] {"WixUIExtension"};
        private readonly string _wixFilesDirectory;
        private readonly WixTools _wixTools;
        private ILogger _logger;

        public MazeWixBuilder(IWixToolRunner wixToolRunner, IMazeRestClient restClient, IFileSystem fileSystem, VersionFolderPathResolver versionFolderPathResolver)
        {
            _wixTools = new WixTools(wixToolRunner);
            _wixToolRunner = wixToolRunner;
            _restClient = restClient;
            _fileSystem = fileSystem;
            _versionFolderPathResolver = versionFolderPathResolver;

            var currentPath = _fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _clientFilesDirectory = _fileSystem.Path.Combine(currentPath, "client");
            _wixFilesDirectory = _fileSystem.Path.Combine(currentPath, "wix");
        }

        public async Task Build(IEnumerable<ClientGroupViewModel> groups, string outputFilename, ILogger logger, CancellationToken cancellationToken)
        {
            _logger = logger;

            if (!_wixToolRunner.IsAvailable)
            {
                logger.LogCritical("Please install WiX Toolset v3.11.1 from this url {url}",
                    "https://github.com/wixtoolset/wix3/releases/download/wix3111rtm/wix311.exe");
                throw new FileNotFoundException($"WiX Tools was not found in {_wixToolRunner.Path}");
            }

            var tempDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(_fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            tempDirectory.Create();
            _logger.LogDebug("Temp directory created at {path}", tempDirectory.FullName);

            try
            {
                var appDataFiles = tempDirectory.CreateSubdirectory("appData");
                var tasks = groups.Select(x => DownloadConfiguration(x, appDataFiles.FullName)).ToList();
                tasks.Add(DownloadConfiguration(null, appDataFiles.FullName));

                var wixFiles = tempDirectory.CreateSubdirectory("wix");
                var releaseComponentsPath = await GenerateClientComponentFile(wixFiles.FullName, cancellationToken);

                var configs = await Task.WhenAll(tasks);
                var startupPackages = GetLoadOnStartupPackages(configs.Where(x => x != null));

                var modulesLock = await DownloadModuleLock(appDataFiles.FullName, startupPackages);

                var packageFiles = await LoadPackages(modulesLock, tempDirectory, cancellationToken);
                var packagesComponent = GeneratePackagesComponentFile(wixFiles.FullName, packageFiles);

                var appDataComponent = await GenerateAppDataComponentFile(appDataFiles.FullName, wixFiles.FullName, cancellationToken);

                var wixMainFiles = _fileSystem.Directory.GetFiles(_wixFilesDirectory, "*.wxs");
                var objFolder = tempDirectory.CreateSubdirectory("obj");
                await _wixTools.Candle.Compile(
                    new Dictionary<string, string> {{"BasePath", _clientFilesDirectory}, {"AppDataPath", appDataFiles.FullName}},
                    _wixExtensions, objFolder.FullName, wixMainFiles.Concat(new[] {releaseComponentsPath, packagesComponent, appDataComponent}),
                    logger, cancellationToken);

                var compilationInfo = new WixLightCompilationInfo
                {
                    OutputFilename = outputFilename,
                    Extensions = _wixExtensions,
                    ObjectFiles = objFolder.GetFiles("*.wixobj").Select(x => x.FullName),
                    LocalizationFiles = _fileSystem.Directory.GetFiles(_wixFilesDirectory, "*.wxl"),
                    SuppressedICEs = new []{ "ICE38", "ICE64" }
                };
                await _wixTools.Light.Compile(compilationInfo, logger, cancellationToken);
            }
            finally
            {
                tempDirectory.Delete(true);
            }
        }

        public async Task<Dictionary<PackageIdentity, FileInfoBase>> LoadPackages(PackagesLock packagesLock, DirectoryInfoBase tempDirectory,
            CancellationToken cancellationToken)
        {
            var result = new Dictionary<PackageIdentity, FileInfoBase>();
            var packagesToDownload = new List<PackageIdentity>();
            foreach (var packageId in packagesLock.Keys)
            {
                var filePath = _versionFolderPathResolver.GetPackageFilePath(packageId.Id, packageId.Version);
                var file = _fileSystem.FileInfo.FromFileName(filePath);
                if (file.Exists)
                {
                    _logger.LogDebug("Package {packageId} could be resolved locally at {path}", packageId, file.FullName);
                    result.Add(packageId, file);
                }
                else
                {
                    _logger.LogDebug("Package {packageId} could not be resolved locally", packageId);
                    packagesToDownload.Add(packageId);
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            if (packagesToDownload.Any())
            {
                _logger.LogInformation("{0} package(s) were not found locally and will be downloaded.", packagesToDownload.Count);

                var nugetFolder = tempDirectory.CreateSubdirectory("nuget");
                var tempFolderPathResolver = new VersionFolderPathResolverFlat(nugetFolder.FullName);

                var serverRepo = new ServerRepository(new Uri(_restClient.Server.ServerUri, "nuget"));
                var downloader = new PackageDownloadManager(new ModulesDirectory(tempFolderPathResolver), serverRepo);

                var results = await downloader.DownloadPackages(
                    new PackagesLock(packagesToDownload.ToDictionary(x => x,
                        _ => (IImmutableList<PackageIdentity>) ImmutableList<PackageIdentity>.Empty)),
                    new PackageDownloadContext(new SourceCacheContext {DirectDownload = true, NoCache = true}, nugetFolder.FullName, true),
                    new NuGetLoggerWrapper(_logger), cancellationToken);

                try
                {
                    foreach (var preFetchResult in results.Values)
                        using (var downloadPackageResult = await preFetchResult.GetResultAsync())
                        {
                            var packageIdentity = await downloadPackageResult.PackageReader.GetIdentityAsync(cancellationToken);
                            _logger.LogInformation("Package {0} was successfully downloaded.", packageIdentity);

                            var fileStream = (FileStream) downloadPackageResult.PackageStream;
                            var file = _fileSystem.FileInfo.FromFileName(fileStream.Name);
                            if (!file.Exists)
                            {
                                _logger.LogError("Package {0} was not found at {1}. The download may have failed.", packageIdentity, file.FullName);
                                throw new FileNotFoundException($"The file {file.FullName} does not exist.");
                            }

                            var packagePath = _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(file.FullName), packageIdentity + ".nupkg");
                            using (var stream = _fileSystem.FileStream.Create(packagePath, FileMode.CreateNew))
                                await fileStream.CopyToAsync(stream);

                            result.Add(packageIdentity, _fileSystem.FileInfo.FromFileName(packagePath));
                        }
                }
                finally
                {
                    foreach (var fetcherResult in results.Values)
                    {
                        await fetcherResult.EnsureResultAsync();
                        fetcherResult.Dispose();
                    }
                }
            }

            return result;
        }

        public async Task<string> GenerateClientComponentFile(string outputDirectory, CancellationToken cancellationToken)
        {
            var filename = _fileSystem.Path.Combine(outputDirectory, ReleaseComponentsFilename);
            await _wixTools.Heat.GenerateComponent(_clientFilesDirectory, "ReleaseComponents", "INSTALLFOLDER", "var.BasePath", filename, _logger,
                cancellationToken);
            return filename;
        }

        public string GeneratePackagesComponentFile(string outputDirectory, Dictionary<PackageIdentity, FileInfoBase> files)
        {
            var filename = _fileSystem.Path.Combine(outputDirectory, PackagesComponentsFilename);
            using (var xmlWriter = XmlWriter.Create(filename, new XmlWriterSettings { Indent = true }))
            {
                var generator = new WixComponentGenerator("PackageComponents", "MazePackages");
                generator.Write(files.Select(x => new WixFile(x.Value.FullName, x.Key.ToString())), xmlWriter);
            }

            return filename;
        }

        public async Task<string> GenerateAppDataComponentFile(string filesDirectory, string outputDirectory, CancellationToken cancellationToken)
        {
            var filename = _fileSystem.Path.Combine(outputDirectory, AppDataComponentsFilename);
            await _wixTools.Heat.GenerateComponent(filesDirectory, "AppDataComponents", "MazeAppData", "var.AppDataPath", filename, _logger,
                cancellationToken);
            return filename;
        }

        public async Task<ClientConfigurationDto> DownloadConfiguration(ClientGroupViewModel group, string outputPath)
        {
            if (group != null)
                _logger.LogInformation("Download client configuration of group {groupId} ({groupName})", group.ClientGroupId, group.Name);
            else _logger.LogInformation("Download global configuration");

            ClientConfigurationDto config;
            try
            {
                config = await ClientConfigurationsResource.GetClientConfiguration(group?.ClientGroupId, _restClient);
            }
            catch (RestNotFoundException)
            {
                _logger.LogWarning("No configuration for {{groupName}} does exist, skip.", group?.Name);
                return null;
            }
            
            var filename = $"mazesettings{group?.ClientGroupId}.json";

            _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(outputPath, filename), config.Content);
            return config;
        }

        public async Task<PackagesLock> DownloadModuleLock(string outputPath, IEnumerable<string> loadOnStartup)
        {
            var modulesLock = await ModulesResource.FetchModules(PrismModule.ClientFramework, _restClient);
            var requiredPackages = new Dictionary<PackageIdentity, IImmutableList<PackageIdentity>>();

            void AddPackages(IEnumerable<PackageIdentity> identities)
            {
                foreach (var identity in identities)
                {
                    if (!requiredPackages.ContainsKey(identity))
                    {
                        var dependencies = modulesLock[identity];
                        requiredPackages.Add(identity, dependencies);
                        AddPackages(dependencies);
                    }
                }
            }

            var loadOnStartupPackages = new List<PackageIdentity>();
            foreach (var module in loadOnStartup)
            {
                var lockedPackage = modulesLock.Where(y => y.Key.Id.Equals(module, StringComparison.OrdinalIgnoreCase));
                if (!lockedPackage.Any())
                {
                    _logger.LogWarning($"The module {module} is not installed on the server and cannot be loaded on startup. Please remove it from the config.");
                    continue;
                }

                loadOnStartupPackages.Add(lockedPackage.First().Key);
            }

            AddPackages(loadOnStartupPackages);

            var requiredPackagesLock = new PackagesLock(requiredPackages);
            _fileSystem.File.WriteAllText(_fileSystem.Path.Combine(outputPath, "modules.lock"), JsonConvert.SerializeObject(requiredPackagesLock));

            return requiredPackagesLock;
        }

        public static IEnumerable<string> GetLoadOnStartupPackages(IEnumerable<ClientConfigurationDto> configs)
        {
            var result = new HashSet<string>();
            foreach (var config in configs)
            {
                dynamic configuration = JObject.Parse(config.Content);
                var loadOnStartup = configuration.Modules.LoadOnStartup;
                if (loadOnStartup == null)
                    continue;

                foreach (var item in loadOnStartup)
                {
                    result.Add(item.Name);
                }
            }

            return result;
        }
    }
}