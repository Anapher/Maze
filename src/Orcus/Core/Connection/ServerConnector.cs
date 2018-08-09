using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Orcus.Core.Rest.Authentication.V1;
using Orcus.Core.Services;
using Orcus.Sockets;

namespace Orcus.Core.Connection
{
    public interface IServerConnector
    {
        Task<ServerConnection> TryConnect(Uri uri);
    }

    public class ServerConnector : IServerConnector
    {
        private readonly IClientInfoProvider _clientInfoProvider;
        private readonly OrcusSocketOptions _options;
        private readonly IOrcusRestClientFactory _restClientFactory;

        public ServerConnector(IOrcusRestClientFactory restClientFactory, IClientInfoProvider clientInfoProvider,
            IOptions<OrcusSocketOptions> options)
        {
            _restClientFactory = restClientFactory;
            _clientInfoProvider = clientInfoProvider;
            _options = options.Value;
        }

        public async Task<ServerConnection> TryConnect(Uri uri)
        {
            var restClient = _restClientFactory.Create(uri);
            var result =
                await AuthenticationResource.Authenticate(_clientInfoProvider.GetAuthenticationDto(), restClient);
            restClient.SetAuthenticated(result.Jwt);

            return new ServerConnection(restClient, result.Modules, _options);
        }
    }
}