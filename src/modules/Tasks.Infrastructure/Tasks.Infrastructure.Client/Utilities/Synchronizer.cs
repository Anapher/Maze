using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Tasks.Infrastructure.Client.Utilities
{
    public class Synchronizer<T>
    {
        private readonly int _initialCount;
        private readonly int _maxCount;
        private readonly ConcurrentDictionary<T, SemaphoreSlim> _locks;

        public Synchronizer() : this(1, 1)
        {
        }

        public Synchronizer(int initialCount, int maxCount)
        {
            _initialCount = initialCount;
            _maxCount = maxCount;
            _locks = new ConcurrentDictionary<T, SemaphoreSlim>();
        }

        public SemaphoreSlim this[T index]
        {
            get
            {
                return _locks.GetOrAdd(index, arg => new SemaphoreSlim(_initialCount, _maxCount));
            }
        }

        public Task LockAsync(T value)
        {
            return this[value].WaitAsync();
        }

        public Task LockAsync(T value, CancellationToken cancellationToken)
        {
            return this[value].WaitAsync(cancellationToken);
        }

        public void Release(T value)
        {
            this[value].Release();
        }
    }

    public class ReaderWriterSynchronizer<T>
    {
        private readonly ConcurrentDictionary<T, AsyncReaderWriterLock> _locks;

        public ReaderWriterSynchronizer()
        {
            _locks = new ConcurrentDictionary<T, AsyncReaderWriterLock>();
        }

        public AsyncReaderWriterLock this[T index]
        {
            get
            {
                return _locks.GetOrAdd(index, arg => new AsyncReaderWriterLock());
            }
        }

        public AwaitableDisposable<IDisposable> ReaderLockAsync(T value)
        {
            return this[value].ReaderLockAsync();
        }

        public AwaitableDisposable<IDisposable> ReaderLockAsync(T value, CancellationToken cancellationToken)
        {
            return this[value].ReaderLockAsync(cancellationToken);
        }

        public IDisposable ReaderLock(T value)
        {
            return this[value].ReaderLock();
        }

        public IDisposable ReaderLock(T value, CancellationToken cancellationToken)
        {
            return this[value].ReaderLock(cancellationToken);
        }

        public AwaitableDisposable<IDisposable> WriterLockAsync(T value)
        {
            return this[value].WriterLockAsync();
        }

        public AwaitableDisposable<IDisposable> WriterLockAsync(T value, CancellationToken cancellationToken)
        {
            return this[value].WriterLockAsync(cancellationToken);
        }

        public IDisposable WriterLock(T value)
        {
            return this[value].WriterLock();
        }

        public IDisposable WriterLock(T value, CancellationToken cancellationToken)
        {
            return this[value].WriterLock(cancellationToken);
        }
    }
}
