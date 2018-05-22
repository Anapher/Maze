using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Hubs;
using Orcus.Server.Service.ModulesV1;
using Orcus.Server.Service.ModulesV1.Config;
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
        public IActionResult GetAll([FromServices] IModuleManager moduleManager)
        {
            return Ok(moduleManager.LoadedModules.Select(x => x.Dto).ToList());
        }

        [HttpPost("installModules"), ValidateModelState]
        public async Task<IActionResult> InstallModule([FromBody] SourcedPackageIdentity packageIdentity,
            [FromServices] IModuleManager moduleManager, [FromServices] ILogger<ModulesController> logger)
        {
            if (string.IsNullOrWhiteSpace(packageIdentity.Id))
                return BadRequest();

            await moduleManager.InstallModule(packageIdentity, new NuGetLoggerWrapper(logger));
            await _hubContext.Clients.All.InvokeAsync("ModuleInstalled", packageIdentity);

            return Ok();
        }

        [HttpGet("sources")]
        public IActionResult GetSources([FromServices] IRepositorySourceConfig repositorySourceConfig)
        {
            return Ok(repositorySourceConfig.Items);
        }

        [HttpPost("sources")]
        [ValidateModelState]
        [Authorize("installModules")]
        public async Task<IActionResult> AddSource([FromBody] Uri sourceRepository,
            [FromServices] IRepositorySourceConfig repositorySourceConfig)
        {
            if (sourceRepository == null)
                return BadRequest();

            await repositorySourceConfig.AddItem(sourceRepository);
            await _hubContext.Clients.All.InvokeAsync("RepositorySourceAdded", sourceRepository);
            return Ok();
        }

        [HttpDelete("sources")]
        [ValidateModelState]
        [Authorize("installModules")]
        public async Task<IActionResult> DeleteSource([FromBody] Uri sourceRepository,
            [FromServices] IRepositorySourceConfig repositorySourceConfig)
        {
            if (sourceRepository == null)
                return BadRequest();

            if (repositorySourceConfig.Items.All(x => x != sourceRepository))
                return NotFound();

            await repositorySourceConfig.RemoveItem(sourceRepository);
            await _hubContext.Clients.All.InvokeAsync("RepositorySourceRemoved", sourceRepository);
            return Ok();
        }
    }
}