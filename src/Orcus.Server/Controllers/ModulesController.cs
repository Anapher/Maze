using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NuGet.Packaging.Core;
using Orcus.Server.Hubs;
using Orcus.Server.Service.Modules;
using Orcus.Server.Service.Modules.PackageManagement;
using Orcus.Server.Utilities;

namespace Orcus.Server.Controllers
{
    [Route("v1/[controller]")]
    [Authorize("admin")]
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

        [HttpPost("installModules"), ValidateModelState]
        public async Task<IActionResult> InstallModule([FromBody] PackageIdentity packageIdentity,
            [FromServices] IModulePackageManager moduleManager, [FromServices] ILogger<ModulesController> logger)
        {
            if (string.IsNullOrWhiteSpace(packageIdentity.Id))
                return BadRequest();

            await moduleManager.PreviewInstallPackageAsync(packageIdentity, new ResolutionContext(), new NuGetLoggerWrapper(logger),
                CancellationToken.None);

            await _hubContext.Clients.All.SendAsync("ModuleInstalled", packageIdentity);

            return Ok();
        }

        [HttpGet("sources")]
        public IActionResult GetSources([FromServices] IModuleProject project)
        {
            return Ok(project.PrimarySources.Select(x => x.PackageSource.SourceUri).ToList());
        }
    }
}