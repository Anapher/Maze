using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Orcus.Server.Connection.Utilities;
using Orcus.Server.Library.Hubs;
using Orcus.Server.Service.Modules;
using Orcus.Utilities;

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
        public async Task<IActionResult> GetAll([FromQuery] string framework, [FromServices] IModulePackageManager modulePackageManager)
        {
            if (string.IsNullOrWhiteSpace(framework))
                return BadRequest("Framework cannot be empty");

            var nugetFramework = NuGetFramework.Parse(framework);
            var modules = await modulePackageManager.GetPackagesLock(nugetFramework);
            return Ok(modules);
        }

        [HttpGet("installed")]
        public IActionResult GetInstalledModules([FromServices] IVirtualModuleManager moduleManager) => Ok(moduleManager.GetModules());

        [HttpPost]
        public async Task<IActionResult> InstallModule([FromBody] PackageIdentity packageIdentity, [FromServices] IVirtualModuleManager moduleManager)
        {
            if (string.IsNullOrWhiteSpace(packageIdentity?.Id))
                return BadRequest("Package cannot be empty");

            moduleManager.InstallPackage(packageIdentity);

            await _hubContext.Clients.All.SendAsync("ModuleInstalled", PackageIdentityConvert.ToString(packageIdentity));
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveModule([FromQuery] string package, [FromServices] IVirtualModuleManager moduleManager)
        {
            var packageIdentity = PackageIdentityConvert.ToPackageIdentity(package);
            if (string.IsNullOrWhiteSpace(packageIdentity?.Id))
                return BadRequest();

            moduleManager.UninstallPackage(packageIdentity);

            await _hubContext.Clients.All.SendAsync("ModuleUninstalled", package);
            return Ok();
        }

        [HttpGet("sources")]
        public IActionResult GetSources([FromServices] IModuleProject project)
        {
            return Ok(project.PrimarySources.Select(x => x.PackageSource.SourceUri).ToList());
        }

        [HttpPost("package")]
        public async Task<IActionResult> GetPackageInfo([FromBody] List<PackageIdentity> packages, [FromServices] IModuleProject moduleProject)
        {
            var resourceAsync = await moduleProject.LocalSourceRepository.GetResourceAsync<PackageMetadataResource>();
            var context = new SourceCacheContext();

            var metadata = await TaskCombinators.ThrottledAsync(packages,
                (identity, token) => resourceAsync.GetMetadataAsync(identity, context, NullLogger.Instance, token), HttpContext.RequestAborted);

            return Ok(metadata.Select(Mapper.Map<PackageSearchMetadata>).ToList());
        }
    }
}