using System.Threading.Tasks;
using Console.Shared.Channels;
using Orcus.Administration.ControllerExtensions;
using Orcus.Administration.Library.Clients;

namespace Console.Administration.Rest
{
    public class ConsoleResource : ChannelResource<ConsoleResource>
    {
        public ConsoleResource() : base(null)
        {
        }

        public static async Task<ProcessInterface> ProcessWatcher(IPackageRestClient restClient) =>
            new ProcessInterface(await restClient.CreateChannel<ConsoleResource, IProcessChannel>("processChannel"));
    }
}