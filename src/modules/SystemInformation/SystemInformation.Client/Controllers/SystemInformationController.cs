using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;

namespace SystemInformation.Client.Controllers
{
    public class SystemInformationController : OrcusController
    {
        [OrcusGet]
        public IActionResult TestAction()
        {

        }
    }
}