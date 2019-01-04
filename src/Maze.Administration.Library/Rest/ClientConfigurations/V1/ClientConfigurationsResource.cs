using System;
using System.Threading.Tasks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;
using Maze.Server.Connection.Clients;

namespace Maze.Administration.Library.Rest.ClientConfigurations.V1
{
    public class ClientConfigurationsResource : VersionedResource<ClientConfigurationsResource>
    {
        public ClientConfigurationsResource() : base("clients/configuration")
        {
        }

        public static Task<ClientConfigurationDto> GetClientConfiguration(int? groupId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, groupId).Execute(restClient).Return<ClientConfigurationDto>();

        public static Task PutClientConfiguration(ClientConfigurationDto clientConfiguration, IRestClient restClient) =>
            CreateRequest(HttpVerb.Put, clientConfiguration.ClientGroupId, clientConfiguration).Execute(restClient);

        public static Task PostClientConfiguration(ClientConfigurationDto clientConfiguration, IRestClient restClient) =>
            CreateRequest(HttpVerb.Post, clientConfiguration.ClientGroupId ?? throw new ArgumentException("Configuration must be a group config"),
                clientConfiguration).Execute(restClient);

        public static Task DeleteClientConfiguration(int groupId, IRestClient restClient) => CreateRequest(HttpVerb.Delete, groupId).Execute(restClient);
    }
}