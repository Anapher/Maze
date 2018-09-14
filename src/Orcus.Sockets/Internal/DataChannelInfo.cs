using System;
using System.Reflection;
using Orcus.Modules.Api;

namespace Orcus.Sockets.Internal
{
    internal struct DataChannelInfo : IDisposable
    {
        public DataChannelInfo(IDataChannel dataChannel, int channelId)
        {
            DataChannel = dataChannel;
            ChannelId = channelId;
            IsSynchronized = dataChannel.GetType().GetCustomAttribute<SynchronizedChannelAttribute>() != null;
            AsyncLock = new FifoAsyncLock();
        }

        public void Dispose()
        {
            DataChannel?.Dispose();
            AsyncLock?.Dispose();
        }

        public IDataChannel DataChannel { get; set; }
        public int ChannelId { get; set; }
        public bool IsSynchronized { get; set; }
        public FifoAsyncLock AsyncLock { get; set; }
    }
}