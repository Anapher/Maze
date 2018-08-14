using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Orcus.Server.Options;

namespace Orcus.Server.Controllers
{
    //http://localhost:50485/nuget/testmodule/1.0.0/testmodule.1.0.0.nupkg
    [Route("nuget")]
    public class NuGetController : Controller
    {
        private readonly ModulesOptions _options;

        public NuGetController(IOptions<ModulesOptions> options)
        {
            _options = options.Value;
        }

        [Route("{moduleName}/{version}/{filename}")]
        public IActionResult DownloadModule(string moduleName, string version, string filename)
        {
            if (moduleName.Any(x => !char.IsLetterOrDigit(x) && x != '.'))
                return BadRequest();

            var packageIdentity = new PackageIdentity(moduleName, NuGetVersion.Parse(version));

            var modulesDirectory = _options.Directory;
            var moduleFile = Path.Combine(modulesDirectory, packageIdentity.Id.ToLowerInvariant(),
                packageIdentity.Version.ToString().ToLowerInvariant(),
                packageIdentity.ToString().ToLowerInvariant() + ".nupkg");

            if (!System.IO.File.Exists(moduleFile))
                return NotFound();

            return File(System.IO.File.Open(moduleFile, FileMode.Open, FileAccess.Read), "application/octet-stream");
        }
    }
}