using Orcus.Server.Library.Clients;
using Orcus.Server.Library.Clients.Helpers;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Common.Server.Rest.V1
{
    public class WakeOnLanResource : ModuleResource<WakeOnLanResource>
    {
        public WakeOnLanResource() : base("Tasks.Common", "wol")
        {
        }

        public static Task WakeOnLan(PhysicalAddress address, IRestClient restClient)
            => CreateRequest(HttpVerb.Get).AddQueryParam("address", address.ToString()).Execute(restClient);
    }
}
