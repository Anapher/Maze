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
    }
}