using System;

namespace Orcus.Sockets.Internal
{
    internal struct SynchronizedDataSocket : IDisposable
    {
        public SynchronizedDataSocket(IDataSocket dataSocket)
        {
            DataSocket = dataSocket;
            FifoAsyncLock = new FifoAsyncLock();
        }

        public IDataSocket DataSocket { get; set; }
        public FifoAsyncLock FifoAsyncLock { get; set; }

        public void Dispose()
        {
            FifoAsyncLock?.Dispose();
        }
    }
}