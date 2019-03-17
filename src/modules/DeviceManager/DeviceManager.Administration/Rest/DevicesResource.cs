using System.Threading.Tasks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;

namespace DeviceManager.Administration.Rest
{
    public class DevicesResource : ResourceBase<DevicesResource>
    {
        public DevicesResource() : base("devices")
        {
        }

        public static Task EnableDevice(string deviceId, ITargetedRestClient client) =>
            CreateRequest(HttpVerb.Get, $"{deviceId}/enable").Execute(client);

        public static Task DisableDevice(string deviceId, ITargetedRestClient client) =>
            CreateRequest(HttpVerb.Get, $"{deviceId}/disable").Execute(client);
    }
}