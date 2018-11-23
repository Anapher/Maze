using Orcus.Server.Library.Clients;
using Orcus.Server.Library.Clients.Helpers;
using System.Threading.Tasks;

namespace Tasks.Common.Server.Rest.V1
{
    public class PowerResource : ModuleResource<PowerResource>
    {
        public PowerResource() : base("Tasks.Common", "system/power")
        {
        }

        public static Task Shutdown(IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "shutdown").Execute(restClient);
    }
}
