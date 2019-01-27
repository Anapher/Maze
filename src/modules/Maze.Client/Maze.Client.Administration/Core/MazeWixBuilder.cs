using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Models;
using Maze.Administration.Library.Rest.ClientConfigurations.V1;
using Maze.Administration.Library.Rest.Modules.V1;
using Maze.Client.Administration.Core.Wix;
using Maze.Server.Connection.Modules;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NuGet.Frameworks;

namespace Maze.Client.Administration.Core
{
    public class MazeWixBuilder
    {
        private readonly WixTools _wixTools;
        private readonly IMazeRestClient _restClient;
        private ILogger _logger;

        private readonly string _clientFilesDirectory;
        private readonly string _wixFilesDirectory;
        private readonly string _packagesDirectory;

        private const string ReleaseComponentsFilename = "Components.Release.Generated.wxs";
        private const string AppDataComponentsFilename = "Components.AppData.Generated.wxs";

        public MazeWixBuilder(WixTools wixTools, IMazeRestClient restClient)
        {
            _wixTools = wixTools;
            _restClient = restClient;

            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _clientFilesDirectory = Path.Combine(currentPath, "client");
            _wixFilesDirectory = Path.Combine(currentPath, "wix");

            _packagesDirectory = "packages"; //TODO update
        }

        public async Task Build(ILogger logger, IEnumerable<ClientGroupViewModel> groups)
        {
            _logger = logger;

            var tempDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            tempDirectory.Create();
            _logger.LogDebug("Temp directory created at {path}", tempDirectory.FullName);

            try
            {
                var appDataFiles = tempDirectory.CreateSubdirectory("appData");
                var tasks = groups.Select(x => DownloadConfiguration(x, appDataFiles.FullName)).ToList();
                tasks.Add(DownloadConfiguration(null, appDataFiles.FullName));

                var wixFiles = tempDirectory.CreateSubdirectory("wix");
                var releaseComponentsPath = await GenerateClientComponentFile(wixFiles.FullName);

                var modulesLock = await DownloadModuleLock(appDataFiles.FullName);

            }
            finally
            {
                
            }
        }

        private async Task<string> GenerateClientComponentFile(string outputDirectory)
        {
            var filename = Path.Combine(outputDirectory, ReleaseComponentsFilename);
            await _wixTools.Heat.GenerateComponent(_clientFilesDirectory, "ReleaseComponents", "INSTALLFOLDER", "var.BasePath", filename, _logger);
            return filename;
        }

        private async Task<string> GenerateAppDataComponentFile(string filesDirectory, string outputDirectory)
        {
            var filename = Path.Combine(outputDirectory, AppDataComponentsFilename);
            await _wixTools.Heat.GenerateComponent(filesDirectory, "AppDataComponents", "MazeAppData", "var.AppDataPath", filename, _logger);
            return filename;
        }

        private async Task DownloadConfiguration(ClientGroupViewModel group, string outputPath)
        {
            if (group != null)
                _logger.LogInformation("Download client configuration of group {groupId} ({groupName})", group.ClientGroupId, group.Name);
            else _logger.LogInformation("Download global configuration");

            var config = await ClientConfigurationsResource.GetClientConfiguration(group?.ClientGroupId, _restClient);
            var filename = $"mazesettings{group?.ClientGroupId}.json";

            File.WriteAllText(Path.Combine(outputPath, filename), config.Content);
        }

        private async Task<PackagesLock> DownloadModuleLock(string outputPath)
        {
            var modulesLock = await ModulesResource.FetchModules(PrismModule.ClientFramework, _restClient);
            File.WriteAllText(Path.Combine(outputPath, "modules.lock"), JsonConvert.SerializeObject(modulesLock));

            return modulesLock;
        }
    }
}
