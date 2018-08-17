using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Sockets
{
    /// <summary>
    ///     A websocket like implementation that provides the methods to send prefixed buffers over a stream
    /// </summary>
    public class OrcusSocket : IDataSocket
    {
        //   7 6 5 4 3 2 1 0
        //  +-+-+-+-+-+-+-+-+
        //  | | | | | | | | |
        //  +-+-+-+-+-+-+-+-+
        //   ^ ^ ^ ^     ^ ^
        //   | | | |     | |
        //   \ | | /     | +-- IS_FINISHED
        //      |        +---- IS_CONTINUATION
        //      +------------- TYPE
        /// 
        public enum MessageOpcode : byte
        {
            Message = 0x1,
            Close = 0x2,
            Request = 0x3,
            Response = 0x4,
            Ping = 0x9,
            Pong = 0xA,

            RequestSinglePackage = Request | OpCodeModifier.IsFinished,
            RequestContinuation = Request | OpCodeModifier.IsContinuation,
            RequestContinuationFinished = Request | OpCodeModifier.IsFinished | OpCodeModifier.IsContinuation,

            ResponseSinglePackage = Response | OpCodeModifier.IsFinished,
            ResponseContinuation = Response | OpCodeModifier.IsContinuation,
            ResponseContinuationFinished = Response | OpCodeModifier.IsFinished | OpCodeModifier.IsContinuation
        }

        private const int MaxMessageHeaderLength = 6;

        /// <summary>
        ///     CancellationTokenSource used to abort all current and future operations when anything is canceled or any error
        ///     occurs.
        /// </summary>
        private readonly CancellationTokenSource _abortSource = new CancellationTokenSource();

        /// <summary>Timer used to send periodic pings to the server, at the interval specified</summary>
        private readonly Timer _keepAliveTimer;

        /// <summary>Buffer used for reading data from the network.</summary>
        private readonly byte[] _receiveBuffer = new byte[2];

        /// <summary>Semaphore used to ensure that calls to SendFrameAsync don't run concurrently.</summary>
        private readonly SemaphoreSlim _sendFrameAsyncLock = new SemaphoreSlim(1, 1);

        private readonly Stream _stream;

        /// <summary>true if Dispose has been called; otherwise, false.</summary>
        private bool _disposed;

        /// <summary>The number of bytes available in the _receiveBuffer.</summary>
        private int _receiveBufferCount;

        /// <summary>The offset of the next available byte in the _receiveBuffer.</summary>
        private int _receiveBufferOffset;

        /// <summary>
        ///     Initialize a new instance of <see cref="OrcusSocket" />
        /// </summary>
        /// <param name="stream">The stream that carries the underlying connection</param>
        /// <param name="keepAliveInterval">
        ///     The frequency the ping-pong messages should be send to verify that the underlying
        ///     connection is still alive
        /// </param>
        public OrcusSocket(Stream stream, TimeSpan? keepAliveInterval)
        {
            _stream = stream;

            // Set up the abort source so that if it's triggered, we transition the instance appropriately.
            _abortSource.Token.Register(s =>
            {
                var thisRef = (OrcusSocket) s;

                lock (thisRef.StateUpdateLock)
                {
                    var state = thisRef.State;
                    if (state != WebSocketState.Closed && state != WebSocketState.Aborted)
                        thisRef.State = state != WebSocketState.None && state != WebSocketState.Connecting
                            ? WebSocketState.Aborted
                            : WebSocketState.Closed;
                }
            }, this);

            if (keepAliveInterval.HasValue)
                _keepAliveTimer = new Timer(s => ((OrcusSocket) s).SendKeepAliveFrameAsync(), this,
                    keepAliveInterval.Value, keepAliveInterval.Value);
        }

        /// <summary>Lock used to protect update and check-and-update operations on _state.</summary>
        private object StateUpdateLock => _abortSource;

        public WebSocketState State { get; private set; }

        public void Dispose()
        {
            lock (StateUpdateLock)
            {
                DisposeCore();
            }
        }

        private void DisposeCore()
        {
            Debug.Assert(Monitor.IsEntered(StateUpdateLock), $"Expected {nameof(StateUpdateLock)} to be held");
            if (!_disposed)
            {
                _disposed = true;
                _keepAliveTimer?.Dispose();
                _stream?.Dispose();
                if (State < WebSocketState.Aborted) State = WebSocketState.Closed;
            }
        }

        public ArrayPool<byte> BufferPool { get; } = ArrayPool<byte>.Shared;
        public event EventHandler<DataReceivedEventArgs> DataReceivedEventArgs;

        public void Abort()
        {
            _abortSource.Cancel();
            Dispose(); // forcibly tear down connection
        }

        public async Task ReceiveAsync()
        {
            while (true)
            {
                await EnsureBufferContainsAsync(2);
                var opcode = (MessageOpcode) _receiveBuffer[0];
                var length = ReadPayloadLength(_receiveBuffer[1]);

                ArraySegment<byte> receiveBuffer = default;
                if (length > 0)
                {
                    var buffer = ArrayPool<byte>.Shared.Rent(length);
                    var count = length;

                    var numRead = 0;
                    do
                    {
                        var n = _stream.Read(buffer, numRead, count);
                        if (n == 0)
                            break;
                        numRead += n;
                        count -= n;
                    } while (count > 0);

                    if (numRead != length)
                        ThrowIfEofUnexpected(true);

                    receiveBuffer = new ArraySegment<byte>(buffer, 0, length);
                }

                if (opcode == MessageOpcode.Ping || opcode == MessageOpcode.Pong)
                {
                    await HandleReceivedPingPongAsync();
                    continue;
                }

                if (opcode == MessageOpcode.Close)
                {
                }

                DataReceivedEventArgs?.Invoke(this, new DataReceivedEventArgs(receiveBuffer, opcode));
                _receiveBufferCount = 0;
            }
        }

        private int ReadPayloadLength(byte firstByte)
        {
            var readNextByteFromStream = false;

            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            var count = 0;
            var shift = 0;
            byte b;

            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7) // 5 bytes max per Int32, shift += 7
                    throw new FormatException("Invalid 7 Bit encoded integer");

                if (readNextByteFromStream)
                {
                    b = ReadByteSafe();
                }
                else
                {
                    b = firstByte;
                    readNextByteFromStream = true;
                }

                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);

            return count;
        }

        /// <summary>Sends a websocket frame to the network.</summary>
        /// <param name="opcode">The opcode for the message.</param>
        /// <param name="payloadBuffer">The buffer containing the payload data fro the message.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the websocket.</param>
        public Task SendFrameAsync(MessageOpcode opcode, ArraySegment<byte> payloadBuffer,
            CancellationToken cancellationToken) =>
            cancellationToken.CanBeCanceled || !_sendFrameAsyncLock.Wait(0)
                ? SendFrameFallbackAsync(opcode, payloadBuffer, cancellationToken)
                : SendFrameLockAcquiredNonCancelableAsync(opcode, payloadBuffer);

        /// <summary>Sends a websocket frame to the network. The caller must hold the sending lock.</summary>
        /// <param name="opcode">The opcode for the message.</param>
        /// <param name="payloadBuffer">The buffer containing the payload data fro the message.</param>
        private Task SendFrameLockAcquiredNonCancelableAsync(MessageOpcode opcode, ArraySegment<byte> payloadBuffer)
        {
            // If we get here, the cancellation token is not cancelable so we don't have to worry about it,
            // and we own the semaphore, so we don't need to asynchronously wait for it.
            Task writeTask = default;
            var releaseSemaphoreAndSendBuffer = true;
            var sendBuffer = WriteFrameToSendBuffer(opcode, payloadBuffer);

            try
            {
                // Write the payload synchronously to the buffer, then write that buffer out to the network.
                writeTask = _stream.WriteAsync(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count);

                // If the operation happens to complete synchronously (or, more specifically, by
                // the time we get from the previous line to here), release the semaphore, return
                // the task, and we're done.
                if (writeTask.IsCompleted)
                    return writeTask;

                // Up until this point, if an exception occurred (such as when accessing _stream or when
                // calling GetResult), we want to release the semaphore and the send buffer. After this point,
                // both need to be held until writeTask completes.
                releaseSemaphoreAndSendBuffer = false;
            }
            catch (Exception exc)
            {
                return Task.FromException(exc is OperationCanceledException ? exc :
                    State == WebSocketState.Aborted ? CreateOperationCanceledException(exc) :
                    new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc));
            }
            finally
            {
                if (releaseSemaphoreAndSendBuffer)
                {
                    _sendFrameAsyncLock.Release();
                    ReleaseSendBuffer(sendBuffer.Array);
                }
            }

            // The write was not yet completed. Create and return a continuation that will
            // release the semaphore and translate any exception that occurred.
            return writeTask.ContinueWith((t, s) =>
            {
                var thisRef = (OrcusSocket) s;
                thisRef._sendFrameAsyncLock.Release();
                thisRef.ReleaseSendBuffer(sendBuffer.Array);

                try
                {
                    t.GetAwaiter().GetResult();
                }
                catch (Exception exc)
                {
                    throw thisRef.State == WebSocketState.Aborted
                        ? CreateOperationCanceledException(exc)
                        : new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc);
                }
            }, this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private static int WriteHeader(MessageOpcode opcode, byte[] sendBuffer, ReadOnlyMemory<byte> payload)
        {
            sendBuffer[0] = (byte) opcode;

            var offset = 1;
            // Store the payload length.

            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            var v = (uint) payload.Length; // support negative numbers
            while (v >= 0x80)
            {
                sendBuffer[offset++] = (byte) (v | 0x80);
                v >>= 7;
            }

            sendBuffer[offset++] = (byte) v;

            return offset;
        }

        /// <summary>Writes a frame into the send buffer, which can then be sent over the network.</summary>
        private ArraySegment<byte> WriteFrameToSendBuffer(MessageOpcode opcode, ArraySegment<byte> payloadBuffer)
        {
            var sendBuffer = AllocateSendBuffer(payloadBuffer.Count + MaxMessageHeaderLength);
            var headerLength = WriteHeader(opcode, sendBuffer, payloadBuffer);

            if (payloadBuffer.Count > 0)
                Buffer.BlockCopy(payloadBuffer.Array, payloadBuffer.Offset, sendBuffer, headerLength,
                    payloadBuffer.Count);

            return new ArraySegment<byte>(sendBuffer, 0, headerLength + payloadBuffer.Count);
        }

        private async Task SendFrameFallbackAsync(MessageOpcode opcode, ArraySegment<byte> payloadBuffer,
            CancellationToken cancellationToken)
        {
            var sendBuffer = WriteFrameToSendBuffer(opcode, payloadBuffer);

            await _sendFrameAsyncLock.WaitAsync().ConfigureAwait(false);
            try
            {
                using (cancellationToken.Register(s => ((OrcusSocket) s).Abort(), this))
                {
                    await _stream.WriteAsync(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception exc)
            {
                throw State == WebSocketState.Aborted
                    ? CreateOperationCanceledException(exc, cancellationToken)
                    : new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc);
            }
            finally
            {
                _sendFrameAsyncLock.Release();
                ReleaseSendBuffer(sendBuffer.Array);
            }
        }

        private async Task EnsureBufferContainsAsync(int minimumRequiredBytes, bool throwOnPrematureClosure = true)
        {
            Debug.Assert(minimumRequiredBytes <= _receiveBuffer.Length,
                $"Requested number of bytes {minimumRequiredBytes} must not exceed {_receiveBuffer.Length}");

            // If we don't have enough data in the buffer to satisfy the minimum required, read some more.
            if (_receiveBufferCount < minimumRequiredBytes)
            {
                // If there's any data in the buffer, shift it down.  
                if (_receiveBufferCount > 0)
                    Buffer.BlockCopy(_receiveBuffer, _receiveBufferOffset, _receiveBuffer, 0, _receiveBufferCount);
                _receiveBufferOffset = 0;

                // While we don't have enough data, read more.
                while (_receiveBufferCount < minimumRequiredBytes)
                {
                    var numRead = await _stream
                        .ReadAsync(_receiveBuffer, _receiveBufferCount, _receiveBuffer.Length - _receiveBufferCount)
                        .ConfigureAwait(false);
                    Debug.Assert(numRead >= 0, $"Expected non-negative bytes read, got {numRead}");
                    _receiveBufferCount += numRead;
                    if (numRead == 0)
                    {
                        // The connection closed before we were able to read everything we needed.
                        // If it was due to use being disposed, fail.  If it was due to the connection
                        // being closed and it wasn't expected, fail.  If it was due to the connection
                        // being closed and that was expected, exit gracefully.
                        if (_disposed)
                            throw new ObjectDisposedException(nameof(WebSocket));
                        if (throwOnPrematureClosure)
                            throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
                        break;
                    }
                }
            }
        }

        private void ThrowIfEofUnexpected(bool throwOnPrematureClosure)
        {
            // The connection closed before we were able to read everything we needed.
            // If it was due to us being disposed, fail.  If it was due to the connection
            // being closed and it wasn't expected, fail.  If it was due to the connection
            // being closed and that was expected, exit gracefully.
            if (_disposed) throw new ObjectDisposedException(nameof(WebSocket));
            if (throwOnPrematureClosure) throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
        }

        private byte ReadByteSafe()
        {
            var result = _stream.ReadByte();
            if (result == -1)
                throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);

            return (byte) result;
        }

        private Task HandleReceivedPingPongAsync() =>
            SendFrameAsync(MessageOpcode.Pong, default, CancellationToken.None);

        private void SendKeepAliveFrameAsync()
        {
            var acquiredLock = _sendFrameAsyncLock.Wait(0);
            if (acquiredLock)
            {
                // This exists purely to keep the connection alive; don't wait for the result, and ignore any failures.
                // The call will handle releasing the lock.
                SendFrameLockAcquiredNonCancelableAsync(MessageOpcode.Ping, new ArraySegment<byte>());
            }
        }

        /// <summary>
        ///     Creates an OperationCanceledException instance, using a default message and the specified inner exception and
        ///     token.
        /// </summary>
        private static Exception CreateOperationCanceledException(Exception innerException,
            CancellationToken cancellationToken = default) =>
            new OperationCanceledException(new OperationCanceledException().Message, innerException, cancellationToken);

        private byte[] AllocateSendBuffer(int minLength) => ArrayPool<byte>.Shared.Rent(minLength);

        private void ReleaseSendBuffer(byte[] buffer)
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        [Flags]
        private enum OpCodeModifier
        {
            IsFinished = 0b1000_0000,
            IsContinuation = 0b0100_0000
        }
    }
}