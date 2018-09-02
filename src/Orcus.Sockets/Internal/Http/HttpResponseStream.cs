using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Orcus.Modules.Api.Extensions;
using Orcus.Modules.Api.Request;
using Orcus.Sockets.Internal.Extensions;
using Orcus.Sockets.Internal.Infrastructure;
using Orcus.Sockets.Logging;

namespace Orcus.Sockets.Internal.Http
{
    public class HttpResponseStream : WriteOnlyStream
    {
        private static readonly ILog Logger = LogProvider.For<HttpResponseStream>();

        private readonly DefaultOrcusResponse _response;
        private readonly OrcusRequest _request;
        private readonly IDataSocket _socket;
        private readonly int _packageBufferSize;
        private readonly int _maxHeaderSize;
        private readonly CancellationToken _requestCancellationToken;
        private bool _isFinalPackage;
        private bool _isFinalPackagePushed;
        private ArraySegment<byte> _latestSendBuffer;
        private bool _disposed;
        private bool _isCompressionEnabled;
        private bool _hasSentPackage;

        public HttpResponseStream(DefaultOrcusResponse response, OrcusRequest request, IDataSocket socket,
            int packageBufferSize, int maxHeaderSize, CancellationToken requestCancellationToken)
        {
            _response = response;
            _request = request;
            _socket = socket;
            _packageBufferSize = packageBufferSize;
            _maxHeaderSize = maxHeaderSize;
            _requestCancellationToken = requestCancellationToken;
        }

        public async Task FinalFlushAsync()
        {
            _isFinalPackage = true;

            if (_latestSendBuffer == default)
            {
                //if no new package was written, we must generate an empty package
                await SendData(new ArraySegment<byte>(BitConverter.GetBytes(_response.RequestId), 0, 4));
            }
            else
                await SendData(default);

            _isFinalPackagePushed = true;
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_response.IsFinished)
                throw new InvalidOperationException("The response is already finished and no new data can be written");

            CancellationTokenSource linkedToken = null;
            if (cancellationToken != CancellationToken.None)
            {
                linkedToken =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _requestCancellationToken);
                cancellationToken = linkedToken.Token;
            }

            using (linkedToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!_response.HasStarted)
                {
                    if (!_response.IsCompleted && !_isCompressionEnabled)
                    {
                        if (_request.GetTypedHeaders().AcceptEncoding
                                ?.Contains(new StringWithQualityHeaderValue("gzip")) == true)
                        {
                            _isCompressionEnabled = true;
                            _response.Headers.Add("Content-Encoding", "gzip");

                            //if the response has not been completed, we compress the body
                            var bodyStream = (BufferingWriteStream) _response.Body;
                            var newPackagingStream = new BufferingWriteStream(this, _packageBufferSize);

                            var gzipStream = new GZipStream(newPackagingStream, CompressionLevel.Fastest, false);
                            bodyStream.SetInnerStream(gzipStream);

                            bodyStream.FlushCallback = () => newPackagingStream.FlushAsync();

                            await gzipStream.WriteAsync(buffer, offset, count, cancellationToken);
                            return;
                        }
                    }

                    _response.StartResponse();

                    if (!_response.Headers.ContainsKey(OrcusHeaders.OrcusSocketRequestIdHeader)) //automatically set
                        throw new InvalidOperationException(
                            $"Response must have the header {OrcusHeaders.OrcusSocketRequestIdHeader}");

                    var sendBuffer = ArrayPool<byte>.Shared.Rent(count + _maxHeaderSize);
                    var headerOffset = HttpFormatter.FormatResponse(_response, new ArraySegment<byte>(sendBuffer));
                    if (headerOffset > _maxHeaderSize)
                        throw new InvalidOperationException(
                            $"The header size {headerOffset}B exceeds the maximum allowed header size ({_maxHeaderSize}B)");

                    if (count > 0)
                        Buffer.BlockCopy(buffer, offset, sendBuffer, headerOffset, count);

                    Logger.LogDataPackage("Send HTTP Response", sendBuffer, 0, headerOffset + count);
                    await SendData(new ArraySegment<byte>(sendBuffer, 0, headerOffset + count));
                }
                else
                {
                    var sendBuffer = ArrayPool<byte>.Shared.Rent(count + 4);
                    Buffer.BlockCopy(buffer, offset, sendBuffer, 4, count);
                    BinaryUtils.WriteInt32(ref sendBuffer, 0, _response.RequestId);

                    await SendData(new ArraySegment<byte>(sendBuffer, 0, count + 4));
                }
            }
        }

        private async Task SendData(ArraySegment<byte> data)
        {
            if (_isFinalPackagePushed)
                throw new InvalidOperationException("Cannot push data after the final one");

            if (_disposed)
                throw new ObjectDisposedException("The stream is disposed and no data can be pushed");

            var latestBuffer = _latestSendBuffer;
            _latestSendBuffer = data;

            if (latestBuffer == default)
                return;

            Logger.LogDataPackage("Send HTTP Response Continuation", latestBuffer.Array, latestBuffer.Offset,
                latestBuffer.Count);

            OrcusSocket.MessageOpcode opcode;
            if (_hasSentPackage)
                opcode = _isFinalPackage
                    ? OrcusSocket.MessageOpcode.ResponseContinuationFinished
                    : OrcusSocket.MessageOpcode.ResponseContinuation;
            else
                opcode = _isFinalPackage
                    ? OrcusSocket.MessageOpcode.ResponseSinglePackage
                    : OrcusSocket.MessageOpcode.Response;
            
            try
            {
                await _socket.SendFrameAsync(opcode, latestBuffer, CancellationToken.None);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(latestBuffer.Array);
            }

            _hasSentPackage = true;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).Wait();
        }

        public override bool CanSeek { get; } = false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            //dont do anything here as we send all data immediately except the latest package
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;
                if (_latestSendBuffer != default)
                {
                    ArrayPool<byte>.Shared.Return(_latestSendBuffer.Array);
                    _latestSendBuffer = default;
                }
            }
        }
    }
}
