using System.Collections.Generic;
using System.Threading.Tasks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;
using Maze.Server.Connection.Clients;

namespace Maze.Administration.Library.Rest.Clients.V1
{
    public class ClientsResource : VersionedResource<ClientsResource>
    {
        public ClientsResource() : base("clients")
        {
        }

        public static Task<List<ClientDto>> FetchAsync(IRestClient client) => CreateRequest().Execute(client).Return<List<ClientDto>>();
    }
}