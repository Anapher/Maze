using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Maze.Administration.Library.Channels;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Exceptions;
using Maze.Modules.Api;
using Microsoft.AspNetCore.SignalR.Client;

namespace Maze.Administration.Core.Rest
{
    public class MazeRestClientWrapper : IMazeRestClient
    {
        private IMazeRestClient _restClient;

        public MazeRestClientWrapper(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_restClient == null)
                throw new RestClientNotConnectedException();

            return _restClient.SendMessage(request, cancellationToken);
        }

        public void Dispose()
        {
        }

        public string Username
        {
            get
            {
                if (_restClient == null)
                    throw new RestClientNotConnectedException();

                return _restClient.Username;
            }
        }

        public IServerInfo Server
        {
            get
            {
                if (_restClient == null)
                    throw new RestClientNotConnectedException();

                return _restClient.Server;
            }
        }

        public HubConnection HubConnection
        {
            get
            {
                if (_restClient == null)
                    throw new RestClientNotConnectedException();

                return _restClient.HubConnection;
            }
        }

        public IServiceProvider ServiceProvider { get; }

        public Task<TChannel> OpenChannel<TChannel>(HttpRequestMessage message, CancellationToken cancellationToken)
            where TChannel : IAwareDataChannel
        {
            if (_restClient == null)
                throw new RestClientNotConnectedException();

            return _restClient.OpenChannel<TChannel>(message, cancellationToken);
        }

        public Task<HttpResponseMessage> SendChannelMessage(HttpRequestMessage request, IDataChannel channel, CancellationToken cancellationToken)
        {
            if (_restClient == null)
                throw new RestClientNotConnectedException();

            return _restClient.SendChannelMessage(request, channel, cancellationToken);
        }

        public void Initialize(MazeRestClient restClient)
        {
            _restClient = restClient;
            restClient.ServiceProvider = ServiceProvider;
        }
    }
}