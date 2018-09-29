using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;

namespace ClientPanel.Client.Controllers
{
    public class ModuleController : OrcusController
    {
        [OrcusGet]
        public IActionResult TestAction()
        {
            return Ok("Hello World!");
        }
    }
}