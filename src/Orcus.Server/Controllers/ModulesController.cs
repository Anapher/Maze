using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace Orcus.Server.Controllers
{
    [Route("v1/[controller]")]
    public class ModulesController : Controller
    {
        public ModulesController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //https://github.com/NuGet/Home/issues/5674
            return Ok();
        }
    }

    public interface IModuleManager
    {
        IReadOnlyList<PackageIdentity> InstalledModules { get; }

    }
}
