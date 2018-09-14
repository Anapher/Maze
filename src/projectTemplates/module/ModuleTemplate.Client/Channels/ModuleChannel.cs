using ModuleTemplate.Shared.Channels;
using Orcus.ControllerExtensions;
using Orcus.Modules.Api.Routing;

namespace ModuleTemplate.Client.Channels
{
    [Route("testChannel")]
    public class ModuleChannel : CallTransmissionChannel<IChannel>, IChannel
    {
    }
}