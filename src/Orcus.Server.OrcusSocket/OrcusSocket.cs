using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusSocket : IDisposable
    {
        private readonly Stream _stream;
        private const int MaxMessageHeaderLength = 6;

        /// <summary>CancellationTokenSource used to abort all current and future operations when anything is canceled or any error occurs.</summary>
        private readonly CancellationTokenSource _abortSource = new CancellationTokenSource();
        private Memory<byte> _receiveBuffer;

        /// <summary>Lock used to protect update and check-and-update operations on _state.</summary>
        private object StateUpdateLock => _abortSource;

        /// <summary>Timer used to send periodic pings to the server, at the interval specified</summary>
        private readonly Timer _keepAliveTimer;

        /// <summary>Semaphore used to ensure that calls to SendFrameAsync don't run concurrently.</summary>
        private readonly SemaphoreSlim _sendFrameAsyncLock = new SemaphoreSlim(1, 1);

        /// <summary>true if Dispose has been called; otherwise, false.</summary>
        private bool _disposed;

        public OrcusSocket(Stream stream, TimeSpan? keepAliveInterval)
        {
            _stream = stream;

            // Set up the abort source so that if it's triggered, we transition the instance appropriately.
            _abortSource.Token.Register(s =>
            {
                var thisRef = (OrcusSocket) s;

                lock (thisRef.StateUpdateLock)
                {
                    WebSocketState state = thisRef.State;
                    if (state != WebSocketState.Closed && state != WebSocketState.Aborted)
                    {
                        thisRef.State = state != WebSocketState.None && state != WebSocketState.Connecting ?
                            WebSocketState.Aborted :
                            WebSocketState.Closed;
                    }
                }
            }, this);

            if (keepAliveInterval.HasValue)
            {
                _keepAliveTimer = new Timer(s => ((OrcusSocket) s).SendKeepAliveFrameAsync(), this, keepAliveInterval.Value,
                    keepAliveInterval.Value);
            }
        }

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
                if (State < WebSocketState.Aborted)
                {
                    State = WebSocketState.Closed;
                }
            }
        }

        public event EventHandler<HttpRequest> HttpRequestReceived;

        public WebSocketState State { get; private set; }

        private void SendKeepAliveFrameAsync()
        {
            bool acquiredLock = _sendFrameAsyncLock.Wait(0);
            if (acquiredLock)
            {
                // This exists purely to keep the connection alive; don't wait for the result, and ignore any failures.
                // The call will handle releasing the lock.
                SendFrameLockAcquiredNonCancelableAsync(MessageOpcode.Ping, new ArraySegment<byte>());
            }
            else
            {
                // If the lock is already held, something is already getting sent,
                // so there's no need to send a keep-alive ping.
            }
        }

        public void Abort()
        {
            _abortSource.Cancel();
            Dispose(); // forcibly tear down connection
        }

        public async Task SendHttpRequest()
        {

        }

        /// <summary>Sends a websocket frame to the network.</summary>
        /// <param name="opcode">The opcode for the message.</param>
        /// <param name="payloadBuffer">The buffer containing the payload data fro the message.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the websocket.</param>
        private Task SendFrameAsync(MessageOpcode opcode, ArraySegment<byte> payloadBuffer, CancellationToken cancellationToken)
        {
            // If a cancelable cancellation token was provided, that would require registering with it, which means more state we have to
            // pass around (the CancellationTokenRegistration), so if it is cancelable, just immediately go to the fallback path.
            // Similarly, it should be rare that there are multiple outstanding calls to SendFrameAsync, but if there are, again
            // fall back to the fallback path.
            return cancellationToken.CanBeCanceled || !_sendFrameAsyncLock.Wait(0) ?
                SendFrameFallbackAsync(opcode, payloadBuffer, cancellationToken) :
                SendFrameLockAcquiredNonCancelableAsync(opcode, payloadBuffer);
        }

        /// <summary>Sends a websocket frame to the network. The caller must hold the sending lock.</summary>
        /// <param name="opcode">The opcode for the message.</param>
        /// <param name="payloadBuffer">The buffer containing the payload data fro the message.</param>
        private Task SendFrameLockAcquiredNonCancelableAsync(MessageOpcode opcode, ArraySegment<byte> payloadBuffer)
        {
            // If we get here, the cancellation token is not cancelable so we don't have to worry about it,
            // and we own the semaphore, so we don't need to asynchronously wait for it.
            Task writeTask = default;
            bool releaseSemaphoreAndSendBuffer = true;
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
                return Task.FromException(
                    exc is OperationCanceledException ? exc :
                    State == WebSocketState.Aborted ? CreateOperationCanceledException(exc) :
                    new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc));
            }
            finally
            {
                if (releaseSemaphoreAndSendBuffer)
                {
                    _sendFrameAsyncLock.Release();
                }
            }

            // The write was not yet completed.  Create and return a continuation that will
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
            var v = (uint) payload.Length;   // support negative numbers
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
                Buffer.BlockCopy(payloadBuffer.Array, payloadBuffer.Offset, sendBuffer, headerLength, payloadBuffer.Count);

            return new ArraySegment<byte>(sendBuffer, 0, headerLength + payloadBuffer.Count);
        }

        private async Task SendFrameFallbackAsync(MessageOpcode opcode, ArraySegment<byte> payloadBuffer, CancellationToken cancellationToken)
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
                throw State == WebSocketState.Aborted ?
                    CreateOperationCanceledException(exc, cancellationToken) :
                    new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc);
            }
            finally
            {
                _sendFrameAsyncLock.Release();
                ReleaseSendBuffer(sendBuffer.Array);
            }
        }

        public async Task<HttpResponse> CreateEmptyResponse()
        {
            return null;
        }

        /// <summary>Creates an OperationCanceledException instance, using a default message and the specified inner exception and token.</summary>
        private static Exception CreateOperationCanceledException(Exception innerException,
            CancellationToken cancellationToken = default)
        {
            return new OperationCanceledException(new OperationCanceledException().Message, innerException,
                cancellationToken);
        }

        private enum MessageOpcode : byte
        {
            Message = 0x1,
            Request = 0x2,
            Close = 0x8,
            Ping = 0x9,
            Pong = 0xA
        }

        private byte[] AllocateSendBuffer(int minLength)
        {
            return ArrayPool<byte>.Shared.Rent(minLength);
        }

        private void ReleaseSendBuffer(byte[] buffer)
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}