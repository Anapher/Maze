using System.Collections.Generic;
using System.Threading.Tasks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;
using Maze.Server.Connection.Clients;

namespace Maze.Administration.Library.Rest.ClientGroups.V1
{
    public class ClientGroupsResource : VersionedResource<ClientGroupsResource>
    {
        public ClientGroupsResource() : base("clients/groups")
        {
        }

        public static Task<List<ClientGroupDto>> GetGroups(IRestClient client) => CreateRequest().Execute(client).Return<List<ClientGroupDto>>();

        public static Task<ClientGroupDto> GetGroup(int clientGroupId, IRestClient client) =>
            CreateRequest(HttpVerb.Get, clientGroupId).Execute(client).Return<ClientGroupDto>();

        public static Task<ClientGroupDto> PostGroup(ClientGroupDto clientGroupDto, IRestClient client) =>
            CreateRequest(HttpVerb.Post, null, clientGroupDto).Execute(client).CreateFromLocationId(x =>
            {
                clientGroupDto.ClientGroupId = x;
                return clientGroupDto;
            });

        public static Task PutGroup(ClientGroupDto clientGroupDto, IRestClient client) =>
            CreateRequest(HttpVerb.Put, clientGroupDto.ClientGroupId, clientGroupDto).Execute(client);

        public static Task DeleteGroup(int clientGroupId, IRestClient client) => CreateRequest(HttpVerb.Delete, clientGroupId).Execute(client);

        public static Task PostClientsToGroup(int clientGroupId, IEnumerable<int> clients, IRestClient client) =>
            CreateRequest(HttpVerb.Post, clientGroupId + "/add", clients).Execute(client);

        public static Task DeleteClientsFromGroup(int clientGroupId, IEnumerable<int> clients, IRestClient client) =>
            CreateRequest(HttpVerb.Post, clientGroupId + "/remove", clients).Execute(client);
    }
}