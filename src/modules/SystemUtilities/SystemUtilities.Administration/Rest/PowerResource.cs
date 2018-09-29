using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;

namespace SystemUtilities.Administration.Rest
{
    public class PowerResource : ResourceBase<PowerResource>
    {
        public PowerResource() : base($"{PrismModule.ModuleName}/Power")
        {
        }

        public static Task Shutdown(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "shutdown").Execute(restClient);
        public static Task Restart(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "restart").Execute(restClient);
        public static Task LogOff(ITargetedRestClient restClient) => CreateRequest(HttpVerb.Get, "logoff").Execute(restClient);
    }
}