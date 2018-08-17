using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Orcus.Server.Hubs;
using Orcus.Server.Service.Modules;

namespace Orcus.Server.Controllers
{
    [Route("v1/[controller]")]
    [Authorize("admin")]
    [ApiController]
    public class ModulesController : Controller
    {
        private readonly IHubContext<AdministrationHub> _hubContext;

        public ModulesController(IHubContext<AdministrationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string framework,
            [FromServices] IModulePackageManager modulePackageManager)
        {
            var nugetFramework = NuGetFramework.Parse(framework);
            var modules = await modulePackageManager.GetPackagesLock(nugetFramework);
            return Ok(modules);
        }

        [HttpPost("install")]
        public async Task<IActionResult> InstallModule([FromBody] PackageIdentity packageIdentity,
            [FromServices] IModulePackageManager moduleManager)
        {
            if (string.IsNullOrWhiteSpace(packageIdentity.Id))
                return BadRequest();

            await moduleManager.InstallPackageAsync(packageIdentity, moduleManager.GetDefaultResolutionContext(),
                moduleManager.GetDefaultDownloadContext(), CancellationToken.None);

            await _hubContext.Clients.All.SendAsync("ModuleInstalled", packageIdentity);

            return Ok();
        }

        [HttpGet("sources")]
        public IActionResult GetSources([FromServices] IModuleProject project)
        {
            return Ok(project.PrimarySources.Select(x => x.PackageSource.SourceUri).ToList());
        }

        [HttpGet("inst")]
        [AllowAnonymous]
        public async Task<IActionResult> Install([FromServices] IModulePackageManager packageManager)
        {
            await packageManager.InstallPackageAsync(new PackageIdentity("UserInteraction", NuGetVersion.Parse("1.0")),
                packageManager.GetDefaultResolutionContext(), packageManager.GetDefaultDownloadContext(),
                CancellationToken.None);
            return Ok();
        }
    }
}