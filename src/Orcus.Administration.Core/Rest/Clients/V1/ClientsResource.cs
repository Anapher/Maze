using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Administration.Core.Clients;
using Orcus.Administration.Core.Clients.Helpers;
using Orcus.Server.Connection.Clients;

namespace Orcus.Administration.Core.Rest.Clients.V1
{
    public class ClientsResource : VersionedResource<ClientsResource>
    {
        public ClientsResource() : base("clients")
        {
        }

        public static Task<List<ClientDto>> FetchAsync(IOrcusRestClient client)
        {
            return CreateRequest().Execute(client).Return<List<ClientDto>>();
        }
    }
}