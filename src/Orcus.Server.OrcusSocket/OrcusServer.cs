using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api.Request;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusServer
    {
        private const string OrcusSocketRequestIdHeader = "orcussocket-requestid";

        private readonly OrcusSocket _socket;
        private readonly ConcurrentDictionary<uint, OrcusChannel> _channels;
        private readonly ConcurrentDictionary<OrcusChannel, uint> _channelsReversed;
        private int _requestCounter;

        public OrcusServer(OrcusSocket socket)
        {
            _socket = socket;
            _channels = new ConcurrentDictionary<uint, OrcusChannel>();
            _channelsReversed = new ConcurrentDictionary<OrcusChannel, uint>();

            socket.MessageReceived += SocketOnMessageReceived;
            socket.RequestReceived += SocketOnRequestReceived;
        }

        public void RegisterChannel(OrcusChannel channel, uint channelId)
        {
            channel.SendMessage += ChannelOnSendMessage;

            _channels.TryAdd(channelId, channel);
            _channelsReversed.TryAdd(channel, channelId);
        }

        private void SocketOnMessageReceived(object sender, OrcusMessage e)
        {
            _channels[e.ChannelId].InvokeMessage(e);
        }

        private void ChannelOnSendMessage(object sender, ArraySegment<byte> e)
        {
            var channel = (OrcusChannel) sender;
            var channelId = _channelsReversed[channel];
            _socket.SendAsync(new OrcusMessage {ChannelId = channelId, Buffer = e});
        }

        public void StartRequest(OrcusRequest orcusRequest)
        {
            if (orcusRequest.Headers.ContainsKey(OrcusSocketRequestIdHeader))
                throw new ArgumentException($"The orcus request must not have a {OrcusSocketRequestIdHeader} header.",
                    nameof(orcusRequest));

            var requestId = Interlocked.Increment(ref _requestCounter);
            orcusRequest.Headers.Add(OrcusSocketRequestIdHeader, requestId.ToString());

            var buffer = new byte[231];
            //write header#

        }

        private void SocketOnRequestReceived(object sender, ArraySegment<byte> e)
        {

        }

        private void ReceiveRequest(HttpRequest request)
        {

        }
    }
}