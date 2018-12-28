using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Maze.Core.Rest.Authentication.V1;
using Maze.Core.Services;
using Maze.Sockets;

namespace Maze.Core.Connection
{
    public interface IServerConnector
    {
        Task<ServerConnection> TryConnect(Uri uri);
    }

    public class ServerConnector : IServerConnector
    {
        private readonly IClientInfoProvider _clientInfoProvider;
        private readonly MazeSocketOptions _options;
        private readonly IMazeRestClientFactory _restClientFactory;

        public ServerConnector(IMazeRestClientFactory restClientFactory, IClientInfoProvider clientInfoProvider,
            IOptions<MazeSocketOptions> options)
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