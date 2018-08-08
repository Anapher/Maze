using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Connection.Modules;

namespace Orcus.ModuleManagement.PackageManagement
{
    public class PackageDownloadManager
    {
        private readonly IModulesDirectory _modulesDirectory;
        private readonly SourceRepository _sourceRepository;

        public PackageDownloadManager(IModulesDirectory modulesDirectory, SourceRepository sourceRepository)
        {
            _modulesDirectory = modulesDirectory;
            _sourceRepository = sourceRepository;
        }

        public async Task<Dictionary<PackageIdentity, PackagePreFetcherResult>> DownloadPackages(
            PackagesLock packagesLock, PackageDownloadContext downloadContext, ILogger logger, CancellationToken token)
        {
            var packages = packagesLock.Keys.Concat(packagesLock.SelectMany(x => x.Value)).Distinct();
            var toDownload = new Queue<PackageIdentity>();
            var result = new Dictionary<PackageIdentity, PackagePreFetcherResult>();

            foreach (var package in packages)
            {
                if (_modulesDirectory.ModuleExists(package))
                    continue;

                toDownload.Enqueue(package);
            }

            if (toDownload.Any())
            {
                var maxParallelTasks = PackageManagementConstants.DefaultMaxDegreeOfParallelism;

                var downloadResults = new List<PackagePreFetcherResult>(maxParallelTasks);

                while (toDownload.Count > 0)
                {
                    // Throttle tasks
                    if (downloadResults.Count == maxParallelTasks)
                    {
                        // Wait for a task to complete
                        // This will not throw, exceptions are stored in the result
                        await Task.WhenAny(downloadResults.Select(e => e.EnsureResultAsync()));

                        // Remove all completed tasks
                        downloadResults.RemoveAll(e => e.IsComplete);
                    }

                    var package = toDownload.Dequeue();

                    // Download the package if it does not exist in the packages folder already
                    // Start the download task
                    var task = Task.Run(
                        async () => await PackageDownloader.GetDownloadResourceResultAsync(_sourceRepository, package,
                            downloadContext, "NO_GLOBAL_CACHE_DIRECTORY", logger, token), token);

                    var downloadResult = new PackagePreFetcherResult(task, package, _sourceRepository.PackageSource);

                    downloadResults.Add(downloadResult);
                    result.Add(package, downloadResult);
                }
            }

            return result;
        }
    }
}