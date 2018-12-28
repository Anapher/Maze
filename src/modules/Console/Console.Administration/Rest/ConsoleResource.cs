using System.Threading.Tasks;
using Console.Shared.Channels;
using Maze.Administration.ControllerExtensions;
using Maze.Administration.Library.Clients;

namespace Console.Administration.Rest
{
    public class ConsoleResource : ChannelResource<ConsoleResource>
    {
        public ConsoleResource() : base("Console")
        {
        }

        public static async Task<ProcessInterface> ProcessWatcher(ITargetedRestClient restClient) =>
            new ProcessInterface(await restClient.CreateChannel<ConsoleResource, IProcessChannel>("processChannel"));
    }
}