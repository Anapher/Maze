using System;
using System.Threading;
using System.Threading.Tasks;

namespace Maze.Sockets.Internal
{
    //Inspired by https://stackoverflow.com/a/961904/4166138
    public class FifoAsyncLock : IDisposable
    {
        private readonly SemaphoreSlim _lockSemaphore;
        private int _ticketsCount = 0;
        private int _ticketToRide = 1;

        public FifoAsyncLock()
        {
            _lockSemaphore = new SemaphoreSlim(1, 1);
        }

        public void Dispose()
        {
            _lockSemaphore?.Dispose();
        }

        public async Task EnterAsync(CancellationToken cancellationToken)
        {
            var ticket = Interlocked.Increment(ref _ticketsCount);
            while (true)
            {
                await _lockSemaphore.WaitAsync(cancellationToken);

                if (ticket == _ticketToRide)
                    return;
            }
        }

        public Task EnterAsync()
        {
            return EnterAsync(CancellationToken.None);
        }

        public void Enter(CancellationToken cancellationToken)
        {
            var ticket = Interlocked.Increment(ref _ticketsCount);
            while (true)
            {
                _lockSemaphore.Wait(cancellationToken);

                if (ticket == _ticketToRide)
                    return;
            }
        }

        public void Enter()
        {
            Enter(CancellationToken.None);
        }

        public void Release()
        {
            Interlocked.Increment(ref _ticketToRide);
            _lockSemaphore.Release();
        }
    }
}