using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileExplorer.Client.Utilities;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;

namespace FileExplorer.Client.Controllers
{
    public class FileSystemController : OrcusController
    {
        [OrcusGet]
        public async Task<IActionResult> QueryFileEntries([FromQuery] string path)
        {
            var directoryHelper = new DirectoryHelper();
            var entries = await directoryHelper.GetEntries(path);

            return Ok(entries.ToList());
        }

        [OrcusGet("path")]
        public IActionResult ExpandEnvironmentVariables([FromQuery] string path) =>
            Ok(Environment.ExpandEnvironmentVariables(path));

        [OrcusGet("directory")]
        public IActionResult GetDirectory([FromQuery] string path)
        {
            var directory = new DirectoryHelper().GetDirectoryEntry(new DirectoryInfoEx(path));
            return Ok(directory);
        }
    }
}