using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Client.Extensions;
using FileExplorer.Client.Utilities;
using FileExplorer.Shared.Dtos;
using FileExplorer.Shared.Utilities;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using Orcus.Utilities;

namespace FileExplorer.Client.Controllers
{
    [Route("")]
    public class FileExplorerController : OrcusController
    {
        [OrcusGet("root")]
        public async Task<IActionResult> GetRootElements()
        {
            var result = new RootElementsDto();
            var directoryHelper = new DirectoryHelper();

            var tasks = new List<Task>
            {
                Task.Run(() => result.RootDirectories = directoryHelper.GetNamespaceDirectories()),
                Task.Run(() =>
                    result.ComputerDirectory =
                        directoryHelper.GetDirectoryEntry(DirectoryInfoEx.MyComputerDirectory, null)),
                Task.Run(async () => result.ComputerDirectoryEntries =
                    (await directoryHelper.GetEntriesKeepOrder(DirectoryInfoEx.MyComputerDirectory, CancellationToken.None))
                    .ToList())
            };

            await Task.WhenAll(tasks);
            return Ok(result);
        }

        [OrcusPost("pathTree")]
        public async Task<IActionResult> GetPathTree([FromBody] PathTreeRequestDto request)
        {
            var response = new PathTreeResponseDto();
            var directoryHelper = new DirectoryHelper();

            Task entriesTask = null;
            if (request.RequestEntries)
                entriesTask = directoryHelper.GetEntries(request.Path, CancellationToken.None)
                    .ContinueWith(task => response.Entries = task.Result.ToList());

            if (request.RequestedDirectories?.Count > 0)
            {
                var pathDirectories = PathHelper.GetPathDirectories(request.Path).ToList();
                var directories = new ConcurrentDictionary<int, List<DirectoryEntry>>();

                await TaskCombinators.ThrottledAsync(request.RequestedDirectories, async (i, token) =>
                {
                    var directoryPath = pathDirectories[i];
                    directories.TryAdd(i,
                        (await directoryHelper.GetDirectoryEntries(directoryPath, CancellationToken.None)).ToList());
                }, CancellationToken.None);

                response.Directories = directories;
            }

            if (entriesTask != null)
                await entriesTask;

            return Ok(response);
        }
    }
}