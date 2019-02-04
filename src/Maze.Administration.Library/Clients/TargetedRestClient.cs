using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Maze.Administration.Library.Channels;
using Maze.Administration.Library.Extensions;
using Maze.Modules.Api;
using Prism.Ioc;

namespace Maze.Administration.Library.Clients
{
    public class TargetedRestClient : ITargetedRestClient
    {
        private static readonly Uri RelativeBaseUri = new Uri("v1/modules", UriKind.Relative);
        private readonly string _commandTargetHeader;
        private readonly IMazeRestClient _mazeRestClient;
        private readonly List<IDataChannel> _channels;
        private readonly object _channelsLock = new object();
        private readonly CancellationTokenSource _restClientTokenSource;
        private bool _isDisposed;

        public TargetedRestClient(IMazeRestClient mazeRestClient, int clientId)
        {
            _mazeRestClient = mazeRestClient;
            _commandTargetHeader = "C" + clientId;
            _channels = new List<IDataChannel>();
            _restClientTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            _restClientTokenSource.Cancel();
            _restClientTokenSource.Dispose();

            lock (_channelsLock)
            {
                //list must be copied because Dispose may remove the channel from the list
                foreach (var dataChannel in _channels.ToList())
                    dataChannel.Dispose();

                _channels.Clear();
            }
        }

        public string Username => _mazeRestClient.Username;
        public IServerInfo Server => _mazeRestClient.Server;
        public HubConnection HubConnection => _mazeRestClient.HubConnection;
        public IServiceProvider ServiceProvider => _mazeRestClient.ServiceProvider;

        public async Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TargetedRestClient));

            PatchMessage(request);

            if (!cancellationToken.CanBeCanceled)
            {
                return await _mazeRestClient.SendMessage(request, _restClientTokenSource.Token);
            }

            using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _restClientTokenSource.Token))
            {
                return await _mazeRestClient.SendMessage(request, linkedTokenSource.Token);
            }
        }

        public async Task<TChannel> OpenChannel<TChannel>(HttpRequestMessage message, CancellationToken cancellationToken)
            where TChannel : IAwareDataChannel
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TargetedRestClient));

            PatchMessage(message);
            var channel = await _mazeRestClient.OpenChannel<TChannel>(message, cancellationToken);
            channel.CloseChannel += (sender, args) => ChannelOnCloseChannel(channel);

            lock (_channelsLock)
                _channels.Add(channel);

            return channel;
        }

        public Task<HttpResponseMessage> SendChannelMessage(HttpRequestMessage request, IDataChannel channel, CancellationToken cancellationToken)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TargetedRestClient));

            PatchMessage(request);
            return _mazeRestClient.SendChannelMessage(request, channel, cancellationToken);
        }

        private void ChannelOnCloseChannel(IDataChannel channel)
        {
            lock (_channelsLock)
                _channels.Remove(channel);
        }

        private void PatchMessage(HttpRequestMessage request)
        {
            request.Headers.Add("CommandTarget", _commandTargetHeader);

            //request.Uri = TestModule/Controller
            request.RequestUri = UriHelper.CombineRelativeUris(RelativeBaseUri, request.RequestUri);
        }
    }
}