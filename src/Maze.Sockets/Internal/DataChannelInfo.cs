using System;
using System.Reflection;
using System.Threading;
using Orcus.Modules.Api;

namespace Orcus.Sockets.Internal
{
    internal struct DataChannelInfo : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _disposeLock;
        private bool _isDisposed;

        public DataChannelInfo(IDataChannel dataChannel, int channelId)
        {
            DataChannel = dataChannel;
            ChannelId = channelId;
            IsSynchronized = dataChannel.GetType().GetCustomAttribute<SynchronizedChannelAttribute>() != null;
            AsyncLock = new FifoAsyncLock();

            _disposeLock = new object();
            _isDisposed = false;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async void Dispose()
        {
            if (_isDisposed)
                return;

            lock (_disposeLock)
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
            }

            _cancellationTokenSource.Cancel();
            await AsyncLock.EnterAsync(); //prevent deadlocks
            
            DataChannel.Dispose();
            AsyncLock.Dispose();
            _cancellationTokenSource.Dispose();
        }

        public IDataChannel DataChannel { get; set; }
        public int ChannelId { get; set; }
        public bool IsSynchronized { get; set; }
        public FifoAsyncLock AsyncLock { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}