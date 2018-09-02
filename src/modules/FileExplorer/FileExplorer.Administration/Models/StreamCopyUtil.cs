using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorer.Administration.Models
{
    public class StreamCopyUtil
    {
        private readonly byte[] _buffer;
        private readonly TimeSpan _fastUpdateInterval;
        private readonly TimeSpan _slowUpdateInterval;

        private TransferProgressChangedEventArgs _args;
        private long _bytesTransferred;

        private DateTimeOffset _lastFastUpdate;
        private DateTimeOffset _lastSlowUpdate;
        private long _lastUpdateBytesTransferred;

        private long _processedSize;

        /// <summary>
        ///     Initialize a new instance of <see cref="StreamCopyUtil" />
        /// </summary>
        /// <param name="fastUpdateInterval">
        ///     The update interval of absolute values like
        ///     <see cref="TransferProgressChangedEventArgs.Progress" /> and
        ///     <see cref="TransferProgressChangedEventArgs.ProcessedSize" />
        /// </param>
        /// <param name="slowUpdateInterval">
        ///     The update interval of calculated values like
        ///     <see cref="TransferProgressChangedEventArgs.Speed" /> and
        ///     <see cref="TransferProgressChangedEventArgs.EstimatedTime" />
        /// </param>
        /// <param name="buffer">The buffer that is used to copy from one stream to another</param>
        public StreamCopyUtil(TimeSpan fastUpdateInterval, TimeSpan slowUpdateInterval, byte[] buffer)
        {
            _fastUpdateInterval = fastUpdateInterval;
            _slowUpdateInterval = slowUpdateInterval;
            _buffer = buffer;
        }

        /// <summary>
        ///     Initialize a new instance of <see cref="StreamCopyUtil" />
        /// </summary>
        /// <param name="fastUpdateInterval">
        ///     The update interval of absolute values like
        ///     <see cref="TransferProgressChangedEventArgs.Progress" /> and
        ///     <see cref="TransferProgressChangedEventArgs.ProcessedSize" />
        /// </param>
        /// <param name="slowUpdateInterval">
        ///     The update interval of calculated values like
        ///     <see cref="TransferProgressChangedEventArgs.Speed" /> and
        ///     <see cref="TransferProgressChangedEventArgs.EstimatedTime" />
        /// </param>
        /// <param name="bufferSize">The size of the buffer that is used to copy from one stream to another</param>
        public StreamCopyUtil(TimeSpan fastUpdateInterval, TimeSpan slowUpdateInterval, int bufferSize) : this(
            fastUpdateInterval, slowUpdateInterval, new byte[bufferSize])
        {
        }

        /// <summary>
        ///     Initialize a new instance of <see cref="T:FileExplorer.Administration.Models.StreamCopyUtil" /> with default values
        /// </summary>
        public StreamCopyUtil() : this(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(500), 8192)
        {
        }

        /// <summary>
        ///     The total size of all streams that are copied. This property must be set before <see cref="CopyToAsync"/> is used
        /// </summary>
        public long TotalSize { get; set; }

        public event EventHandler<TransferProgressChangedEventArgs> ProgressChanged;

        public async Task CopyToAsync(Stream source, Stream destination, CancellationToken cancellationToken)
        {
            if (TotalSize == 0)
                throw new InvalidOperationException("The TotalSize must be set before this method is called");

            _lastFastUpdate = DateTimeOffset.UtcNow;
            _lastSlowUpdate = DateTimeOffset.UtcNow;

            var buffer = _buffer;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                _bytesTransferred = destination.Position;
                _processedSize += bytesRead;

                UpdateProgressCallback();
            }
        }

        private void UpdateProgressCallback()
        {
            var now = DateTimeOffset.UtcNow;
            var diff = now - _lastFastUpdate;
            if (diff > _fastUpdateInterval)
                if (ProgressChanged != null)
                {
                    if (_args == null)
                        _args = new TransferProgressChangedEventArgs {TotalSize = TotalSize};

                    var slowUpdateDiff = now - _lastSlowUpdate;
                    if (slowUpdateDiff > _slowUpdateInterval)
                    {
                        _args.Speed = (_bytesTransferred - _lastUpdateBytesTransferred) / slowUpdateDiff.TotalSeconds;

                        var estimatedBytesToTransfer =
                            _processedSize / (double) _bytesTransferred * (TotalSize - _processedSize);
                        _args.EstimatedTime = TimeSpan.FromSeconds(estimatedBytesToTransfer / _args.Speed);

                        _lastUpdateBytesTransferred = _bytesTransferred;
                        _lastSlowUpdate = now;
                    }

                    _args.Progress = _processedSize / (double) TotalSize;
                    _args.ProcessedSize = _processedSize;
                    ProgressChanged?.Invoke(this, _args);

                    _lastFastUpdate = now;
                }
        }
    }
}