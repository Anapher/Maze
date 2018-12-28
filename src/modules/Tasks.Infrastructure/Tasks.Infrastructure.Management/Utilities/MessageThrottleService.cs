using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Maze.Utilities;
using Serilog;

namespace Tasks.Infrastructure.Management.Utilities
{
    public class MessageThrottleService<T> : IDisposable where T : class
    {
        public static readonly ILogger Logger = Log.ForContext<MessageThrottleService<T>>();

        private T _value;
        private readonly object _updateLock = new object();
        private readonly Queue<DateTimeOffset> _lastUpdates;
        private readonly Timer _timer;
        private bool _isTimerEnabled;
        private bool _isDisposed;

        public MessageThrottleService(Func<T, Task> update)
        {
            Update = update;
            _lastUpdates = new Queue<DateTimeOffset>();
            _timer = new Timer(TimerUpdate, null, TimeSpan.Zero, TimeSpan.Zero);
        }

        public void Dispose()
        {
            lock (_updateLock)
            {
                _timer.Dispose();
                _isDisposed = true;
            }
        }

        public Func<T, Task> Update { get; }
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(60);
        public TimeSpan MedianDelay { get; set; } = TimeSpan.FromSeconds(1);

        public async Task SendAsync(T newValue)
        {
            lock (_updateLock)
            {
                if (_isDisposed)
                    return;

                _value = newValue;

                var now = DateTimeOffset.UtcNow;
                while (_lastUpdates.Any() && now > _lastUpdates.Peek().Add(Interval))
                {
                    _lastUpdates.Dequeue();
                }

                var delay = ComputeDelay(MedianDelay, Interval, _lastUpdates.Count);
                if (delay > TimeSpan.Zero)
                {
                    var diffToMostRecent = now - _lastUpdates.Last();
                    if (diffToMostRecent < delay)
                    {
                        if (!_isTimerEnabled)
                            _timer.Change(delay - diffToMostRecent, TimeSpan.Zero);

                        return;
                    }
                }

                _lastUpdates.Enqueue(now);
                _value = default;
            }

            await InvokeUpdate(newValue);
        }

        private void TimerUpdate(object state)
        {
            T value;
            lock (_updateLock)
            {
                if (_isDisposed)
                    return;

                _timer.Change(0, 0);
                _isTimerEnabled = false;

                if (_value == default)
                    return; //e. g. when the SendAsync invokes just before this method

                value = _value;
                _lastUpdates.Enqueue(DateTimeOffset.UtcNow);
            }

            InvokeUpdate(value).Forget();
        }

        private async Task InvokeUpdate(T value)
        {
            try
            {
                await Update(value);
            }
            catch (Exception e)
            {
                Logger.Warning(e, "An error occurred when invoking the update function.");
            }
            Interlocked.CompareExchange(ref _value, default, value);
        }

        public static TimeSpan ComputeDelay(TimeSpan medianDelay, TimeSpan interval, int lastUpdates)
        {
            if (lastUpdates == 0)
                return TimeSpan.Zero;

            var totalExecutionsPerInterval = interval.TotalSeconds / medianDelay.TotalSeconds;
            var delay = LogisticFunction(medianDelay.TotalSeconds, 1 / (totalExecutionsPerInterval / 8), lastUpdates, 0.1);

            return medianDelay + TimeSpan.FromSeconds(delay - (medianDelay.TotalSeconds / 1.6));
        }

        private static double LogisticFunction(double g, double k, double x, double f0)
        {
            return g / (1 + (Math.Pow(Math.E, -k * g * x) * (g / f0 - 1)));
        }
    }
}
