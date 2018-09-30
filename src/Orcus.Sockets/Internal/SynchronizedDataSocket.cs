using System;
using System.Threading;

namespace Orcus.Sockets.Internal
{
    internal struct SynchronizedDataSocket : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _disposeLock;
        private bool _isDisposed;

        public SynchronizedDataSocket(IDataSocket dataSocket)
        {
            DataSocket = dataSocket;
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

            //do not dispose DataSocket!!
            AsyncLock.Dispose();
            _cancellationTokenSource.Dispose();
        }

        public IDataSocket DataSocket { get; set; }
        public FifoAsyncLock AsyncLock { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}