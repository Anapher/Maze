using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Maze.Sockets
{
    public class WebSocketWrapper : IDataSocket
    {
        private readonly int _packageBufferSize;
        private readonly SemaphoreSlim _sendFrameAsyncLock;

        public WebSocketWrapper(WebSocket webSocket, int packageBufferSize)
        {
            WebSocket = webSocket;
            _packageBufferSize = packageBufferSize + 1 + 1000;
            BufferPool = ArrayPool<byte>.Shared;
            _sendFrameAsyncLock = new SemaphoreSlim(1, 1);
        }

        public void Dispose()
        {
            WebSocket.Dispose();
        }

        public event EventHandler<DataReceivedEventArgs> DataReceivedEventArgs;

        public WebSocket WebSocket { get; }
        public ArrayPool<byte> BufferPool { get; }
        public int? RequiredPreBufferLength { get; } = 1;

        public async Task ReceiveAsync()
        {
            while (WebSocket.State == WebSocketState.Open)
            {
                var buffer = BufferPool.Rent(_packageBufferSize);
                var dataRead = 0;

                while (true)
                {
                    var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer, dataRead, buffer.Length - dataRead),
                        CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                        return;

                    dataRead += result.Count;
                    if (!result.EndOfMessage)
                    {
                        if (dataRead == buffer.Length)
                        {
                            var newBufferLength = buffer.Length * 2;
                            var newBuffer = BufferPool.Rent(newBufferLength);

                            Buffer.BlockCopy(buffer, 0, newBuffer, 0, buffer.Length);
                            BufferPool.Return(buffer);

                            buffer = newBuffer;
                        }

                        continue;
                    }

                    var opCode = (MazeSocket.MessageOpcode) buffer[0];
                    var payload = new BufferSegment(buffer, 1, dataRead - 1, BufferPool);

                    DataReceivedEventArgs?.Invoke(this, new DataReceivedEventArgs(payload, opCode));
                    break;
                }
            }
        }

        public Task SendFrameAsync(MazeSocket.MessageOpcode opcode, ArraySegment<byte> payloadBuffer,
            bool bufferHasRequiredLength, CancellationToken cancellationToken) =>
            !_sendFrameAsyncLock.Wait(0)
                ? SendFrameFallbackAsync(opcode, payloadBuffer, bufferHasRequiredLength, cancellationToken)
                : SendFrameLockAcquiredNonCancelableAsync(opcode, payloadBuffer, bufferHasRequiredLength, cancellationToken);

        private Task SendFrameLockAcquiredNonCancelableAsync(MazeSocket.MessageOpcode opcode, ArraySegment<byte> payloadBuffer, bool bufferHasRequiredLength, CancellationToken cancellationToken)
        {
            // If we get here, the cancellation token is not cancelable so we don't have to worry about it,
            // and we own the semaphore, so we don't need to asynchronously wait for it.
            Task writeTask;
            var releaseSemaphoreAndSendBuffer = true;
            var sendBuffer = WriteFrameToSendBuffer(opcode, payloadBuffer, bufferHasRequiredLength);

            try
            {
                // Write the payload synchronously to the buffer, then write that buffer out to the network.
                writeTask = WebSocket.SendAsync(sendBuffer, WebSocketMessageType.Binary, true, cancellationToken);

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
            finally
            {
                if (releaseSemaphoreAndSendBuffer)
                {
                    _sendFrameAsyncLock.Release();

                    if (!bufferHasRequiredLength)
                        ReleaseSendBuffer(sendBuffer.Array);
                }
            }

            return writeTask.ContinueWith((t, s) =>
            {
                var thisRef = (WebSocketWrapper) s;
                thisRef._sendFrameAsyncLock.Release();

                if (!bufferHasRequiredLength)
                    thisRef.ReleaseSendBuffer(sendBuffer.Array);
            }, this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private async Task SendFrameFallbackAsync(MazeSocket.MessageOpcode opcode, ArraySegment<byte> payloadBuffer, bool bufferHasRequiredLength,
            CancellationToken cancellationToken)
        {
            var sendBuffer = WriteFrameToSendBuffer(opcode, payloadBuffer, bufferHasRequiredLength);

            try
            {
                await _sendFrameAsyncLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    await WebSocket.SendAsync(sendBuffer, WebSocketMessageType.Binary, true, cancellationToken);
                }
                finally
                {
                    _sendFrameAsyncLock.Release();
                }
            }
            finally
            {
                if (!bufferHasRequiredLength)
                    ReleaseSendBuffer(sendBuffer.Array);
            }
        }

        /// <summary>Writes a frame into the send buffer, which can then be sent over the network.</summary>
        private ArraySegment<byte> WriteFrameToSendBuffer(MazeSocket.MessageOpcode opcode, ArraySegment<byte> payloadBuffer, bool bufferHasRequiredLength)
        {
            if (bufferHasRequiredLength)
            {
                payloadBuffer.Array[payloadBuffer.Offset - 1] = (byte) opcode;
                return new ArraySegment<byte>(payloadBuffer.Array, payloadBuffer.Offset - 1, payloadBuffer.Count + 1);
            }

            var sendBuffer = AllocateSendBuffer(payloadBuffer.Count + 1);
            sendBuffer[0] = (byte) opcode;

            if (payloadBuffer.Count > 0)
                Buffer.BlockCopy(payloadBuffer.Array, payloadBuffer.Offset, sendBuffer, 1,
                    payloadBuffer.Count);

            return new ArraySegment<byte>(sendBuffer, 0, 1 + payloadBuffer.Count);
        }

        private byte[] AllocateSendBuffer(int minLength) => BufferPool.Rent(minLength);

        private void ReleaseSendBuffer(byte[] buffer)
        {
            BufferPool.Return(buffer);
        }
    }
}