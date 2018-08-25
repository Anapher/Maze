using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.Client.Utilities;
using FileExplorer.Shared.Dtos;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;

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
                    result.ComputerDirectory = directoryHelper.GetDirectoryEntry(DirectoryInfoEx.MyComputerDirectory, null)),
                Task.Run(async () => result.ComputerDirectoryEntries =
                    (await directoryHelper.GetEntries(DirectoryInfoEx.MyComputerDirectory, CancellationToken.None)).ToList())
            };

            await Task.WhenAll(tasks);
            return Ok(result);
        }

        [OrcusPost("pathTree")]
        public async Task<IActionResult> GetPathTree(PathTreeRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}