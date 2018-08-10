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
using Orcus.Modules.Api;
using Orcus.Server.Hubs;
using Orcus.Server.Service;
using Orcus.Server.Service.Extensions;
using Orcus.Server.Service.Modules;
using Orcus.Server.Service.Modules.PackageManagement;
using Orcus.Server.Utilities;
using IActionResult = Microsoft.AspNetCore.Mvc.IActionResult;

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
        public async Task Install([FromServices] ICommandDistributer moduleManager)
        {
            var response = await moduleManager.Execute(Request.ToHttpRequestMessage("/TestModule"),
                new CommandTarget(CommandTargetType.Client) {Id = 1});

            response.CopyToHttpResponse(Response);
        }
    }
}