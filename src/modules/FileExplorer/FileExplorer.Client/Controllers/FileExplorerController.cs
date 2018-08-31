using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public async Task<IActionResult> GetRootElements(CancellationToken cancellationToken)
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
                    (await directoryHelper.GetEntriesKeepOrder(DirectoryInfoEx.MyComputerDirectory, cancellationToken))
                    .ToList())
            };

            await Task.WhenAll(tasks);
            return Ok(result);
        }

        [OrcusPost("pathTree")]
        public async Task<IActionResult> GetPathTree([FromBody] PathTreeRequestDto request, [FromQuery] bool keepOrder,
            CancellationToken cancellationToken)
        {
            var response = new PathTreeResponseDto();
            var directoryHelper = new DirectoryHelper();

            Task<IEnumerable<FileExplorerEntry>> entriesTask = null;
            if (request.RequestEntries)
            {
                entriesTask = keepOrder
                    ? directoryHelper.GetEntriesKeepOrder(request.Path, cancellationToken)
                    : directoryHelper.GetEntries(request.Path, cancellationToken);
            }

            if (request.RequestedDirectories?.Count > 0)
            {
                var pathDirectories = PathHelper.GetPathDirectories(request.Path).ToList();
                var directories = new ConcurrentDictionary<int, List<DirectoryEntry>>();

                await TaskCombinators.ThrottledAsync(request.RequestedDirectories, async (i, token) =>
                {
                    var directoryPath = pathDirectories[i];
                    directories.TryAdd(i,
                        (await directoryHelper.GetDirectoryEntries(directoryPath, cancellationToken)).ToList());
                }, CancellationToken.None);

                response.Directories = directories;
            }

            if (entriesTask != null)
                response.Entries = (await entriesTask).ToList();

            return Ok(response);
        }

        [OrcusPost("upload")]
        public async Task<IActionResult> UploadFile([FromQuery] string path, CancellationToken cancellationToken)
        {
            using (var archive = new ZipArchive(OrcusContext.Request.Body, ZipArchiveMode.Read, true))
            {
                string fullName = Directory.CreateDirectory(path).FullName;
                foreach (var entry in archive.Entries)
                {
                    var entryPath = Path.GetFullPath(Path.Combine(fullName, entry.FullName));
                    if (Path.GetFileName(entryPath).Length == 0)
                    {
                        Directory.CreateDirectory(entryPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
                        using (Stream destination = System.IO.File.Open(entryPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            using (Stream stream = entry.Open())
                                await stream.CopyToAsync(destination);
                        }

                        System.IO.File.SetLastWriteTime(entryPath, entry.LastWriteTime.DateTime);
                    }
                }
            }

            return Ok();
        }
    }
}