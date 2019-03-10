using ModuleTemplate.Shared.Channels;
using Maze.ControllerExtensions;
using Maze.Modules.Api.Routing;

namespace ModuleTemplate.Client.Channels
{
    [Route("testChannel")]
    public class ModuleNamePlaceholderChannel : CallTransmissionChannel<IChannel>, IChannel
    {
    }
}