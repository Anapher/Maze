using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Clients.Helpers;
using Orcus.Server.Connection.Clients;

namespace Orcus.Administration.Library.Rest.ClientGroups.V1
{
    public class ClientGroupsResource : VersionedResource<ClientGroupsResource>
    {
        public ClientGroupsResource() : base("clients/groups")
        {
        }

        public static Task<List<ClientGroupDto>> FetchAsync(IRestClient client) => CreateRequest().Execute(client).Return<List<ClientGroupDto>>();

        public static Task<ClientGroupDto> FetchAsync(int clientGroupId, IRestClient client) =>
            CreateRequest(HttpVerb.Get, clientGroupId).Execute(client).Return<ClientGroupDto>();

        public static Task<ClientGroupDto> CreateAsync(ClientGroupDto clientGroupDto, IRestClient client) =>
            CreateRequest(HttpVerb.Post, null, clientGroupDto).Execute(client).CreateFromLocationId(x =>
            {
                clientGroupDto.ClientGroupId = x;
                return clientGroupDto;
            });

        public static Task UpdateAsync(ClientGroupDto clientGroupDto, IRestClient client) =>
            CreateRequest(HttpVerb.Put, clientGroupDto.ClientGroupId, clientGroupDto).Execute(client);

        public static Task DeleteAsync(int clientGroupId, IRestClient client) => CreateRequest(HttpVerb.Delete, clientGroupId).Execute(client);

        public static Task AddClientsAsync(int clientGroupId, IEnumerable<int> clients, IRestClient client) =>
            CreateRequest(HttpVerb.Post, clientGroupId + "/add", clients).Execute(client);

        public static Task RemoveClientsAsync(int clientGroupId, IEnumerable<int> clients, IRestClient client) =>
            CreateRequest(HttpVerb.Post, clientGroupId + "/remove", clients).Execute(client);
    }
}