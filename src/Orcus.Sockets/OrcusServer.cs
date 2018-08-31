using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Sockets.Internal;
using Orcus.Sockets.Internal.Http;
using Orcus.Sockets.Logging;

namespace Orcus.Sockets
{
    public class OrcusServer : IDisposable
    {
        private static readonly ILog Logger = LogProvider.For<OrcusServer>();

        private const string OrcusSocketRequestIdHeader = "orcussocket-requestid";
        private readonly int _packageBufferSize = 8192;
        private readonly int _maxHeaderSize = 4096;
        private readonly ConcurrentDictionary<int, BufferQueueStream> _activeRequests;
        private readonly ConcurrentDictionary<int, BufferQueueStream> _activeResponses;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<HttpResponseMessage>> _waitingRequests;
        private readonly ConcurrentDictionary<int, OrcusChannel> _channels;
        private readonly ConcurrentDictionary<OrcusChannel, int> _channelsReversed;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _cancellableRequests;

        private readonly IDataSocket _socket;
        private int _requestCounter;

        public OrcusServer(IDataSocket socket, int packageBufferSize, int maxHeaderSize)
        {
            _socket = socket;
            _packageBufferSize = packageBufferSize - 5;
            _maxHeaderSize = maxHeaderSize;

            _channels = new ConcurrentDictionary<int, OrcusChannel>();
            _channelsReversed = new ConcurrentDictionary<OrcusChannel, int>();
            _activeRequests = new ConcurrentDictionary<int, BufferQueueStream>();
            _activeResponses = new ConcurrentDictionary<int, BufferQueueStream>();
            _waitingRequests = new ConcurrentDictionary<int, TaskCompletionSource<HttpResponseMessage>>();
            _cancellableRequests = new ConcurrentDictionary<int, CancellationTokenSource>();

            socket.DataReceivedEventArgs += SocketOnDataReceivedEventArgs;

            if (Logger.IsTraceEnabled())
                socket.DataReceivedEventArgs += (sender, args) =>
                    Logger.Trace("Data received with Message Opcode {opcode}", args.Opcode);
        }

        public event EventHandler<OrcusRequestReceivedEventArgs> RequestReceived;

        public void RegisterChannel(OrcusChannel channel, int channelId)
        {
            channel.SendMessage = SendMessage;

            _channels.TryAdd(channelId, channel);
            _channelsReversed.TryAdd(channel, channelId);
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (requestMessage.Headers.Contains(OrcusSocketRequestIdHeader))
                throw new ArgumentException($"The orcus request must not have a {OrcusSocketRequestIdHeader} header.",
                    nameof(requestMessage));

            var requestId = Interlocked.Increment(ref _requestCounter);
            requestMessage.Headers.Add(OrcusSocketRequestIdHeader, requestId.ToString());

            var requestWaiter = new TaskCompletionSource<HttpResponseMessage>();
            _waitingRequests.TryAdd(requestId, requestWaiter);

            var sendBuffer = ArrayPool<byte>.Shared.Rent(_packageBufferSize);
            var offset = HttpFormatter.FormatRequest(requestMessage, new ArraySegment<byte>(sendBuffer));
            var maxReadLength = sendBuffer.Length - offset;
            var opCode = OrcusSocket.MessageOpcode.Request;

            Stream bodyStream;
            if (requestMessage.Content != null)
                bodyStream = await requestMessage.Content.ReadAsStreamAsync();
            else bodyStream = null;

            using (bodyStream)
            {
                int read;

                if (bodyStream == null) //no body, single package, easy
                {
                    opCode = OrcusSocket.MessageOpcode.RequestSinglePackage;
                    read = 0;
                }
                else
                {
                    //read something
                    read = await bodyStream.ReadAsync(sendBuffer, offset, maxReadLength, cancellationToken);
                    if (read < maxReadLength)
                    {
                        if (read == 0)
                        {
                            //no data in the stream
                            opCode = OrcusSocket.MessageOpcode.RequestSinglePackage;
                        }
                        else
                        {
                            //we read less than requested. check if we already reached the end
                            var read2 = await bodyStream.ReadAsync(sendBuffer, offset + read, maxReadLength - read, cancellationToken);
                            if (read2 == 0)
                                opCode = OrcusSocket.MessageOpcode.RequestSinglePackage;
                            else
                                read += read2;
                        }
                    }
                }

                cancellationToken.ThrowIfCancellationRequested(); //last chance without having to send a cancel package

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await _socket.SendFrameAsync(opCode, new ArraySegment<byte>(sendBuffer, 0, read + offset),
                        cancellationToken);

                    if (opCode == OrcusSocket.MessageOpcode.Request)
                    {
                        BinaryUtils.WriteInt32(ref sendBuffer, 0, requestId);
                        opCode = OrcusSocket.MessageOpcode.RequestContinuation;
                        maxReadLength = sendBuffer.Length - 4;

                        while (true)
                        {
                            read = await bodyStream.ReadAsync(sendBuffer, 4, maxReadLength, cancellationToken);
                            if (read == 0)
                            {
                                opCode = OrcusSocket.MessageOpcode.RequestContinuationFinished;
                            }
                            else if (read < maxReadLength)
                            {
                                var read2 = await bodyStream.ReadAsync(sendBuffer, 4 + read, maxReadLength - read,
                                    cancellationToken);
                                if (read2 == 0)
                                    opCode = OrcusSocket.MessageOpcode.RequestContinuationFinished;
                                else
                                    read += read2;
                            }

                            await _socket.SendFrameAsync(opCode, new ArraySegment<byte>(sendBuffer, 0, 4 + read),
                                cancellationToken);

                            if (opCode == OrcusSocket.MessageOpcode.RequestContinuationFinished)
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    await _socket.SendFrameAsync(OrcusSocket.MessageOpcode.CancelRequest,
                        new ArraySegment<byte>(BitConverter.GetBytes(requestId)),
                        CancellationToken.None); //DO NOT USE THE CANCELLATION TOKEN HERE
                    throw;
                }
            }

            cancellationToken.Register(() =>
            {
                _socket.SendFrameAsync(OrcusSocket.MessageOpcode.CancelRequest,
                    new ArraySegment<byte>(BitConverter.GetBytes(requestId)), CancellationToken.None).Wait();
                requestWaiter.SetCanceled();
            });

            return await requestWaiter.Task;
        }

        public async Task FinishResponse(OrcusRequestReceivedEventArgs e)
        {
            if (e.CancellationToken.IsCancellationRequested)
                return;

            var defaultResponse = (DefaultOrcusResponse) e.Response;
            defaultResponse.IsCompleted = true;
            await defaultResponse.Body.FlushAsync();

            e.Request.Body.Dispose();
        }

        private void SocketOnDataReceivedEventArgs(object sender, DataReceivedEventArgs e)
        {
            switch (e.Opcode)
            {
                case OrcusSocket.MessageOpcode.Message:
                    ProcessMessage(e.Buffer);
                    break;
                case OrcusSocket.MessageOpcode.Request:
                    ProcessRequest(e.Buffer, false);
                    break;
                case OrcusSocket.MessageOpcode.RequestSinglePackage:
                    ProcessRequest(e.Buffer, true);
                    break;
                case OrcusSocket.MessageOpcode.RequestContinuation:
                    AppendRequestData(e.Buffer, false);
                    break;
                case OrcusSocket.MessageOpcode.RequestContinuationFinished:
                    AppendRequestData(e.Buffer, true);
                    break;
                case OrcusSocket.MessageOpcode.Response:
                    ProcessResponse(e.Buffer, false);
                    break;
                case OrcusSocket.MessageOpcode.ResponseSinglePackage:
                    ProcessResponse(e.Buffer, true);
                    break;
                case OrcusSocket.MessageOpcode.ResponseContinuation:
                    AppendResponseData(e.Buffer, false);
                    break;
                case OrcusSocket.MessageOpcode.ResponseContinuationFinished:
                    AppendResponseData(e.Buffer, true);
                    break;
                case OrcusSocket.MessageOpcode.CancelRequest:
                    CancelRequest(e.Buffer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Task SendMessage(OrcusChannel channel, ArraySegment<byte> data)
        {
            var channelId = _channelsReversed[channel];

            var sendBuffer = ArrayPool<byte>.Shared.Rent(data.Count + 4);
            try
            {
                BinaryUtils.WriteInt32(ref sendBuffer, 0, channelId);
                Buffer.BlockCopy(data.Array, data.Offset, sendBuffer, 4, data.Count);

                return _socket.SendFrameAsync(OrcusSocket.MessageOpcode.Message,
                    new ArraySegment<byte>(sendBuffer, 0, data.Count + 4), CancellationToken.None);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sendBuffer);
            }
        }

        private void ProcessMessage(ArraySegment<byte> buffer)
        {
            var channelId = BitConverter.ToInt32(buffer.Array, buffer.Offset);
            if (!_channels.TryGetValue(channelId, out var channel))
            {
                Logger.Error("Received message for channel {channelId} which does not exist.", channelId);
                throw new InvalidOperationException($"Received message for channel {channelId} which does not exist.");
            }

            Task.Run(() =>
                    channel.InvokeMessage(new ArraySegment<byte>(buffer.Array, buffer.Offset + 4, buffer.Count - 4)))
                .ContinueWith(task => ArrayPool<byte>.Shared.Return(buffer.Array));
        }

        private void AppendRequestData(ArraySegment<byte> buffer, bool isCompleted)
        {
            var requestId = BitConverter.ToInt32(buffer.Array, buffer.Offset);

            if (!_activeRequests.TryGetValue(requestId, out var request))
            {
                Logger.Error(
                    "Received request continuation data for request id {requestId} that does not exist. Existing requests: {@existingRequests}",
                    requestId, string.Join(", ", _activeRequests.Keys));

                throw new InvalidOperationException($"Data received for request {requestId}");
            }

            if (isCompleted)
            {
                request.IsCompleted = true;
                _activeRequests.TryRemove(requestId, out _);
            }

            //important: also push the buffer if it's empty, the stream disposes it and the autoresetevent must be set!
            request.PushBuffer(new ArraySegment<byte>(buffer.Array, buffer.Offset + 4, buffer.Count - 4));
        }

        private void ProcessRequest(ArraySegment<byte> buffer, bool isCompleted)
        {
            Logger.Debug("Request received (isCompleted = {isCompleted}), length = {length}",
                isCompleted, buffer.Count);

            var headerLength = HttpFormatter.ParseRequest(buffer, out var request);

            Logger.Trace("Request: {@request}", request);

            var stream = new BufferQueueStream(_socket.BufferPool);
            request.Body = stream;

            stream.PushBuffer(new ArraySegment<byte>(buffer.Array, buffer.Offset + headerLength,
                buffer.Count - headerLength));

            var requestId = int.Parse(request.Headers[OrcusSocketRequestIdHeader]);
            var cancellationTokenSource = _cancellableRequests.GetOrAdd(requestId, i => new CancellationTokenSource());
            var token = cancellationTokenSource.Token;

            if (token.IsCancellationRequested)
            {
                cancellationTokenSource.Dispose();
                _cancellableRequests.TryRemove(requestId, out _);
                return;
            }

            if (isCompleted)
            {
                stream.IsCompleted = true;
            }
            else
            {
                if (_activeRequests.TryAdd(requestId, stream))
                {
                    Logger.Debug("Added request {requestId} to active requests", requestId);
                }
                else
                {
                    Logger.Error(
                        "Adding the request {requestId} to the active requests failed because it already exists.",
                        requestId);
                    throw new InvalidOperationException("The request already exists, duplicate request id deteceted.");
                }
            }

            var response = new DefaultOrcusResponse(requestId);
            response.Headers.Add(OrcusSocketRequestIdHeader, requestId.ToString());
            response.Body = new PackagingBufferStream(data => SendResponsePackage(response, data, token), _packageBufferSize);

            Task.Run(() => RequestReceived?.Invoke(this, new OrcusRequestReceivedEventArgs(request, response, token)));
        }

        private async Task SendResponsePackage(DefaultOrcusResponse orcusResponse, ArraySegment<byte> data, CancellationToken cancellationToken)
        {
            if (orcusResponse.IsFinished)
                throw new InvalidOperationException("The response is already finished and no new data can be written");

            cancellationToken.ThrowIfCancellationRequested();

            if (!orcusResponse.HasStarted)
            {
                orcusResponse.StartResponse();

                if (!orcusResponse.Headers.ContainsKey(OrcusSocketRequestIdHeader)) //automatically set
                    throw new InvalidOperationException($"Response must have the header {OrcusSocketRequestIdHeader}");

                var sendBuffer = ArrayPool<byte>.Shared.Rent(data.Count + _maxHeaderSize);
                try
                {
                    var offset = HttpFormatter.FormatResponse(orcusResponse, new ArraySegment<byte>(sendBuffer));
                    if (offset > _maxHeaderSize)
                        throw new InvalidOperationException(
                            $"The header size {offset}B exceeds the maximum allowed header size ({_maxHeaderSize}B)");

                    if (data.Count > 0)
                        Buffer.BlockCopy(data.Array, data.Offset, sendBuffer, offset, data.Count);

                    var opCode = OrcusSocket.MessageOpcode.Response;
                    if (orcusResponse.IsCompleted)
                        opCode = OrcusSocket.MessageOpcode.ResponseSinglePackage;

                    LogDataPackage("Send HTTP Response", sendBuffer, 0, offset + data.Count);
                    await _socket.SendFrameAsync(opCode, new ArraySegment<byte>(sendBuffer, 0, offset + data.Count),
                        CancellationToken.None);

                    if (opCode == OrcusSocket.MessageOpcode.ResponseSinglePackage)
                        orcusResponse.Finished();
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(sendBuffer);
                }
            }
            else
            {
                var buffer = ArrayPool<byte>.Shared.Rent(data.Count + 4);
                var opcode = orcusResponse.IsCompleted
                    ? OrcusSocket.MessageOpcode.ResponseContinuationFinished
                    : OrcusSocket.MessageOpcode.ResponseContinuation;

                try
                {
                    Buffer.BlockCopy(data.Array, data.Offset, buffer, 4, data.Count);
                    BinaryUtils.WriteInt32(ref buffer, 0, orcusResponse.RequestId);

                    LogDataPackage("Send HTTP Response Continuation", buffer, 0, data.Count + 4);
                    await _socket.SendFrameAsync(opcode, new ArraySegment<byte>(buffer, 0, data.Count + 4),
                        CancellationToken.None);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                if (opcode == OrcusSocket.MessageOpcode.ResponseContinuationFinished)
                    orcusResponse.Finished();
            }
        }

        private static void LogDataPackage(string name, byte[] buffer, int offset, int count)
        {
            if (Logger.IsDebugEnabled())
            {
                var hash = HashHelper.HashData(buffer, offset, count);
                var s = Encoding.UTF8.GetString(buffer, offset, count);
                Logger.Debug(name + " [{size} => {hash}]:\r\n{data}", count, hash, "");
            }
        }

        private void ProcessResponse(ArraySegment<byte> buffer, bool isCompleted)
        {
            LogDataPackage("Received Response", buffer.Array, buffer.Offset, buffer.Count);
            
            var headerLength = HttpFormatter.ParseResponse(buffer, out var response, out var contentHeaders);
            var requestId = int.Parse(response.Headers.GetValues(OrcusSocketRequestIdHeader).First());
            var bufferSegment =
                new ArraySegment<byte>(buffer.Array, buffer.Offset + headerLength, buffer.Count - headerLength);

            if (isCompleted)
            {
                response.Content = new RawStreamContent(new ArrayPoolMemoryStream(bufferSegment, _socket.BufferPool));

                foreach (var contentHeader in contentHeaders)
                    response.Content.Headers.Add(contentHeader.Key, (IEnumerable<string>) contentHeader.Value);
            }
            else
            {
                var stream = new BufferQueueStream(_socket.BufferPool);
                stream.PushBuffer(bufferSegment);
                if (_activeResponses.TryAdd(requestId, stream))
                {
                    Logger.Debug("Added response {requestId} to active responses", requestId);
                }
                else
                {
                    Logger.Error(
                        "Adding the response {requestId} to the active response failed because it already exists.",
                        requestId);
                    throw new InvalidOperationException("The response already exists, duplicate response id deteceted.");
                }

                response.Content = new RawStreamContent(stream);
            }

            if (!_waitingRequests.TryRemove(requestId, out var taskCompletionSource))
            {
                Logger.Error("No TaskCompletionSource for request {requestId} found.", requestId);
                response.Dispose();
                return;
            }

            taskCompletionSource.SetResult(response);
        }

        private void AppendResponseData(ArraySegment<byte> buffer, bool isCompleted)
        {
            LogDataPackage("Received Response Continuation", buffer.Array, buffer.Offset, buffer.Count);

            var requestId = BitConverter.ToInt32(buffer.Array, buffer.Offset);

            if (!_activeResponses.TryGetValue(requestId, out var response))
            {
                Logger.Error(
                    "Received response continuation data for request id {requestId} that does not exist. Existing responses: {@existingResponses}",
                    requestId, string.Join(", ", _activeRequests.Keys));

                throw new InvalidOperationException($"Data received for response {requestId}");
            }

            if (isCompleted)
            {
                response.IsCompleted = true;
                _activeResponses.TryRemove(requestId, out _);
            }

            //important: also push the buffer if it's empty, the stream disposes it and the autoresetevent must be set!
            response.PushBuffer(new ArraySegment<byte>(buffer.Array, buffer.Offset + 4, buffer.Count - 4));
        }

        private void CancelRequest(ArraySegment<byte> buffer)
        {
            var requestId = BitConverter.ToInt32(buffer.Array, buffer.Offset);
            if (_cancellableRequests.TryGetValue(requestId, out var cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                _cancellableRequests.TryRemove(requestId, out _);
            }
            else
            {
                cancellationTokenSource = _cancellableRequests.GetOrAdd(requestId, i => new CancellationTokenSource());
                cancellationTokenSource.Cancel();

                //important to prevent memory leak as the request may never come in
                Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(task =>
                {
                    cancellationTokenSource.Dispose();
                    _cancellableRequests.TryRemove(requestId, out _);
                });
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}