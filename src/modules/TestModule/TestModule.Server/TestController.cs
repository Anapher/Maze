using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;

namespace TestModule.Server
{
    public class TestController : OrcusController
    {
        [OrcusGet]
        public IActionResult Test() => Ok("Hello World");
    }
}