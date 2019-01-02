using System.Collections.Generic;
using System.Threading.Tasks;
using Maze.Client.Library.Clients;
using Maze.Client.Library.Clients.Helpers;
using Maze.Server.Connection.Clients;

namespace Maze.Core.Rest.ClientConfiguration.V1
{
    public class ClientConfigurationResource : VersionedResource<ClientConfigurationResource>
    {
        public ClientConfigurationResource() : base("clients/configuration")
        {
        }

        public static Task<List<int>> GetConfigurationsInfo(IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "current/info").Execute(restClient).Return<List<int>>();

        public static Task<List<ClientConfigurationDataDto>> GetConfigurations(IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, "current").Execute(restClient).Return<List<ClientConfigurationDataDto>>();
    }
}