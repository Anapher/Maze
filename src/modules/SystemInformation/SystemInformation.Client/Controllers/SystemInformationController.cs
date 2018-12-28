using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemInformation.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Maze.Modules.Api;
using Maze.Modules.Api.Parameters;
using Maze.Modules.Api.Routing;
using Maze.Utilities;

namespace SystemInformation.Client.Controllers
{
    public class SystemInformationController : MazeController
    {
        private readonly ILogger<SystemInformationController> _logger;

        public SystemInformationController(ILogger<SystemInformationController> logger)
        {
            _logger = logger;
        }

        [MazeGet]
        public async Task<IActionResult> GetSystemInfo([FromServices] IEnumerable<ISystemInfoProvider> systemInfoProviders)
        {
            var info = new ConcurrentBag<SystemInfoDto>();
            var result = await TaskCombinators.ThrottledCatchErrorsAsync(systemInfoProviders, (provider, token) => Task.Run(() =>
            {
                foreach (var systemInfoDto in provider.FetchInformation())
                    info.Add(systemInfoDto);
            }), MazeContext.RequestAborted);

            foreach (var error in result)
                _logger.LogDebug(error.Value, "Exception occurrred when invoking {service}", error.Key.GetType().FullName);

            return Ok(info);
        }
    }
}