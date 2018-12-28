using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Maze.Modules.Api;
using Maze.Modules.Api.Parameters;
using Maze.Modules.Api.Routing;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Client.Encoder;
using RemoteDesktop.Shared.Dtos;

namespace RemoteDesktop.Client.Controllers
{
    [Route("")]
    public class RemoteDesktopController : MazeController
    {
        [MazeGet("parameters")]
        public IActionResult GetParameters([FromServices] IEnumerable<IScreenCaptureService> captureServices,
            [FromServices] IEnumerable<IStreamEncoder> encoders)
        {
            var parameters = new ParametersDto
            {
                CaptureServices = captureServices.Where(x => x.IsPlatformSupported).Select(x => x.Id).ToArray(),
                Encoders = encoders.Where(x => x.IsPlatformSupported).Select(x => x.Id).ToArray(),
                Screens = Screen.AllScreens.Select(x => new ScreenDto {Bounds = x.Bounds, IsPrimary = x.Primary}).ToArray()
            };

            return Ok(parameters);
        }
    }
}