using System;
using System.Threading.Tasks;
using Orcus.Core.Rest.Authentication.V1;
using Orcus.Core.Services;

namespace Orcus.Core.Connection
{
    public interface IServerConnector
    {
        Task<ServerConnection> TryConnect(Uri uri);
    }

    public class ServerConnector : IServerConnector
    {
        private readonly IClientInfoProvider _clientInfoProvider;
        private readonly IOrcusRestClientFactory _restClientFactory;

        public ServerConnector(IOrcusRestClientFactory restClientFactory, IClientInfoProvider clientInfoProvider)
        {
            _restClientFactory = restClientFactory;
            _clientInfoProvider = clientInfoProvider;
        }

        public async Task<ServerConnection> TryConnect(Uri uri)
        {
            var restClient = _restClientFactory.Create(uri);
            var result =
                await AuthenticationResource.Authenticate(_clientInfoProvider.GetAuthenticationDto(), restClient);
            restClient.SetAuthenticated(result.Jwt);

            return new ServerConnection(restClient, result.Modules);
        }
    }
}