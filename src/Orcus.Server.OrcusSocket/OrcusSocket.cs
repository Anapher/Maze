using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusMessage : IDisposable
    {
        private bool _disposed;

        public ArraySegment<byte> Buffer { get; set; }
        public uint ChannelId { get; set; }

        public void Dispose()
        {
            if (_disposed)
            {
                ArrayPool<byte>.Shared.Return(Buffer.Array);
                _disposed = true;
            }
        }
    }

    public abstract class OrcusChannel : IDisposable
    {
        public event EventHandler<ArraySegment<byte>> SendMessage;

        public abstract void InvokeMessage(OrcusMessage message);

        public virtual void Dispose()
        {
        }
    }

    public class OrcusServer
    {
        private readonly OrcusSocket _socket;
        private readonly ConcurrentDictionary<uint, OrcusChannel> _channels;
        private readonly ConcurrentDictionary<OrcusChannel, uint> _channelsReversed;

        public OrcusServer(OrcusSocket socket)
        {
            _socket = socket;
            _channels = new ConcurrentDictionary<uint, OrcusChannel>();
            _channelsReversed = new ConcurrentDictionary<OrcusChannel, uint>();

            socket.MessageReceived += SocketOnMessageReceived;
            socket.RequestReceived += SocketOnRequestReceived;
        }

        private void SocketOnMessageReceived(object sender, OrcusMessage e)
        {
            _channels[e.ChannelId].InvokeMessage(e);
        }

        public void RegisterChannel(OrcusChannel channel, uint channelId)
        {
            channel.SendMessage += ChannelOnSendMessage;

            _channels.TryAdd(channelId, channel);
            _channelsReversed.TryAdd(channel, channelId);
        }

        private void ChannelOnSendMessage(object sender, ArraySegment<byte> e)
        {
            var channel = (OrcusChannel) sender;
            var channelId = _channelsReversed[channel];
            _socket.SendAsync(new OrcusMessage {ChannelId = channelId, Buffer = e});
        }

        private void SocketOnRequestReceived(object sender, ArraySegment<byte> e)
        {

        }

        private void ReceiveRequest(HttpRequest request)
        {

        }
    }

    public class OrcusSocket : IDisposable
    {
        private readonly Stream _stream;
        private const int MaxMessageHeaderLength = 6;

        /// <summary>CancellationTokenSource used to abort all current and future operations when anything is canceled or any error occurs.</summary>
        private readonly CancellationTokenSource _abortSource = new CancellationTokenSource();

        /// <summary>Lock used to protect update and check-and-update operations on _state.</summary>
        private object StateUpdateLock => _abortSource;

        /// <summary>Timer used to send periodic pings to the server, at the interval specified</summary>
        private readonly Timer _keepAliveTimer;

        /// <summary>Semaphore used to ensure that calls to SendFrameAsync don't run concurrently.</summary>
        private readonly SemaphoreSlim _sendFrameAsyncLock = new SemaphoreSlim(1, 1);

        /// <summary>true if Dispose has been called; otherwise, false.</summary>
        private bool _disposed;

        /// <summary>Buffer used for reading data from the network.</summary>
        private byte[] _receiveBuffer;

        /// <summary>The offset of the next available byte in the _receiveBuffer.</summary>
        private int _receiveBufferOffset;

        /// <summary>The number of bytes available in the _receiveBuffer.</summary>
        private int _receiveBufferCount;

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
        
        public event EventHandler<OrcusMessage> MessageReceived;
        public event EventHandler<ArraySegment<byte>> RequestReceived;

        public WebSocketState State { get; private set; }

        public void Abort()
        {
            _abortSource.Cancel();
            Dispose(); // forcibly tear down connection
        }

        public Task SendAsync(OrcusMessage message)
        {
            return SendFrameAsync(MessageOpcode.Message, message.Buffer, CancellationToken.None);
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
                    
                    int numRead = 0;
                    do
                    {
                        int n = _stream.Read(buffer, numRead, count);
                        if (n == 0)
                            break;
                        numRead += n;
                        count -= n;
                    } while (count > 0);

                    if (numRead != length)
                        ThrowIfEofUnexpected(throwOnPrematureClosure: true);

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

                if (opcode == MessageOpcode.Message)
                {
                    var channelId = BitConverter.ToUInt32(receiveBuffer.Array, receiveBuffer.Offset);
                    MessageReceived?.Invoke(this,
                        new OrcusMessage
                        {
                            ChannelId = channelId,
                            Buffer = new ArraySegment<byte>(receiveBuffer.Array, receiveBuffer.Offset + 4,
                                receiveBuffer.Count - 4)
                        });
                }

                switch (opcode)
                {
                    case MessageOpcode.Request:
                        RequestReceived?.Invoke(this, receiveBuffer);
                        break;
                    case MessageOpcode.RequestContinuation:
                        break;
                    case MessageOpcode.Response:
                        break;
                    case MessageOpcode.ResponseContinuation:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private int ReadPayloadLength(byte firstByte)
        {
            var readNextByteFromStream = false;

            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            int count = 0;
            int shift = 0;
            byte b;

            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                    throw new FormatException("Invalid 7 Bit encoeded integer");

                if (readNextByteFromStream)
                    b = ReadByteSafe();
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

        private async Task EnsureBufferContainsAsync(int minimumRequiredBytes, bool throwOnPrematureClosure = true)
        {
            Debug.Assert(minimumRequiredBytes <= _receiveBuffer.Length, $"Requested number of bytes {minimumRequiredBytes} must not exceed {_receiveBuffer.Length}");

            // If we don't have enough data in the buffer to satisfy the minimum required, read some more.
            if (_receiveBufferCount < minimumRequiredBytes)
            {
                // If there's any data in the buffer, shift it down.  
                if (_receiveBufferCount > 0)
                {
                    Buffer.BlockCopy(_receiveBuffer, _receiveBufferOffset, _receiveBuffer, 0, _receiveBufferCount);
                }
                _receiveBufferOffset = 0;

                // While we don't have enough data, read more.
                while (_receiveBufferCount < minimumRequiredBytes)
                {
                    int numRead = await _stream.ReadAsync(_receiveBuffer, _receiveBufferCount,
                        _receiveBuffer.Length - _receiveBufferCount).ConfigureAwait(false);
                    Debug.Assert(numRead >= 0, $"Expected non-negative bytes read, got {numRead}");
                    _receiveBufferCount += numRead;
                    if (numRead == 0)
                    {
                        // The connection closed before we were able to read everything we needed.
                        // If it was due to use being disposed, fail.  If it was due to the connection
                        // being closed and it wasn't expected, fail.  If it was due to the connection
                        // being closed and that was expected, exit gracefully.
                        if (_disposed)
                        {
                            throw new ObjectDisposedException(nameof(WebSocket));
                        }
                        else if (throwOnPrematureClosure)
                        {
                            throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
                        }
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
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(WebSocket));
            }
            if (throwOnPrematureClosure)
            {
                throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
            }
        }

        private byte ReadByteSafe()
        {
            var result = _stream.ReadByte();
            if (result == -1)
                throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);

            return (byte) result;
        }

        private Task HandleReceivedPingPongAsync()
        {
            return SendFrameAsync(MessageOpcode.Pong, default, CancellationToken.None);
        }

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
            RequestContinuation = 0x3,
            Response = 0x4,
            ResponseContinuation = 0x5,
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