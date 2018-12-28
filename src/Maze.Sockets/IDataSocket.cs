using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Maze.Sockets
{
    public interface IDataSocket : IDisposable
    {
        /// <summary>
        ///     The array pool that is used to allocate the buffers for <see cref="DataReceivedEventArgs" />. If the buffer given
        ///     to <see cref="DataReceivedEventArgs" /> is no longer in use, it must be returned. This proeprty can be null meaning
        ///     that no array pool is used.
        /// </summary>
        ArrayPool<byte> BufferPool { get; }

        /// <summary>
        ///     An offset <see cref="SendFrameAsync" />.payloadBuffer must have so no new buffer must be allocated to send. If this
        ///     property is respected, <see cref="SendFrameAsync" />.bufferHasRequiredLength must be set to true.
        /// </summary>
        int? RequiredPreBufferLength { get; }

        /// <summary>
        ///     The event that occurres when data was received. This event executes synchronously on the reading thread, meaning
        ///     that this must only block as long as neccessary. The buffer must be returned to <see cref="BufferPool"/>
        /// </summary>
        event EventHandler<DataReceivedEventArgs> DataReceivedEventArgs;

        /// <summary>
        ///     Method to send data to the remote side
        /// </summary>
        /// <param name="opcode">The opcode that the package should have</param>
        /// <param name="payloadBuffer">The buffer array of data that should be sent</param>
        /// <param name="bufferHasRequiredLength">
        ///     If set to true, the <see cref="payloadBuffer" /> has an offset of
        ///     <see cref="RequiredPreBufferLength" /> which can be used for metadata so no new buffer must be allocated. Please
        ///     note that the <see cref="ArraySegment{T}.Offset" /> should still be the offset of the actual data
        /// </param>
        /// <param name="cancellationToken"></param>
        Task SendFrameAsync(MazeSocket.MessageOpcode opcode, ArraySegment<byte> payloadBuffer,
            bool bufferHasRequiredLength, CancellationToken cancellationToken);
    }
}