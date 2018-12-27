using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;
using Orcus.Server.Connection.Clients;

namespace Orcus.Administration.Library.Rest.Clients.V1
{
    public class ClientsResource : VersionedResource<ClientsResource>
    {
        public ClientsResource() : base("clients")
        {
        }

        public static Task<List<ClientDto>> FetchAsync(IRestClient client) => CreateRequest().Execute(client).Return<List<ClientDto>>();
    }
}