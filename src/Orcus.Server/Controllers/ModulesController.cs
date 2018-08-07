using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Orcus.Server.Hubs;
using Orcus.Server.Service.Modules;
using Orcus.Server.Service.Modules.PackageManagement;
using Orcus.Server.Utilities;

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
        public IActionResult GetAll([FromServices] IModuleProject project)
        {
            return Ok(project.PrimaryPackages);
        }

        [HttpPost("install")]
        public async Task<IActionResult> InstallModule([FromBody] PackageIdentity packageIdentity,
            [FromServices] IModulePackageManager moduleManager, [FromServices] ILogger<ModulesController> logger)
        {
            if (string.IsNullOrWhiteSpace(packageIdentity.Id))
                return BadRequest();

            await moduleManager.InstallPackageAsync(packageIdentity, new ResolutionContext(),
                new PackageDownloadContext(new SourceCacheContext{DirectDownload = true, NoCache = true}, "tmp", true), new NuGetLoggerWrapper(logger),
                CancellationToken.None);

            await _hubContext.Clients.All.SendAsync("ModuleInstalled", packageIdentity);

            return Ok();
        }

        [HttpGet("sources")]
        public IActionResult GetSources([FromServices] IModuleProject project)
        {
            return Ok(project.PrimarySources.Select(x => x.PackageSource.SourceUri).ToList());
        }

        [HttpGet("install"), AllowAnonymous]
        public Task<IActionResult> Install([FromServices] IModulePackageManager moduleManager, [FromServices] ILogger<ModulesController> logger)
        {
            return InstallModule(new PackageIdentity("TestModule", NuGetVersion.Parse("1.0")), moduleManager,
                logger);
        }
    }
}