using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;

namespace PowerUserTools.ActiveConnections
{
    [Route("[module]")]
    public class ActiveConnectionsModule : OrcusModule
    {
        [OrcusGet]
        public IActionResult GetConnections()
        {
            return Ok(null);
        }
    }
}