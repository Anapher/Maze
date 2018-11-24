using System;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Sockets.Internal;

namespace Tasks.Infrastructure.Client.Utilities
{
    public class TaskQueue
    {
        private readonly FifoAsyncLock _semaphore;
        private bool _isDisposed;

        public TaskQueue()
        {
            _semaphore = new FifoAsyncLock();
        }

        public async Task DisposeAsync()
        {
            await _semaphore.EnterAsync();
            _semaphore.Dispose();
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TaskQueue));

            await _semaphore.EnterAsync();

            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TaskQueue));

            try
            {
                return await taskGenerator();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Enqueue(Func<Task> taskGenerator)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TaskQueue));

            await _semaphore.EnterAsync();

            if (_isDisposed)
                throw new ObjectDisposedException(nameof(TaskQueue));

            try
            {
                await taskGenerator();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}