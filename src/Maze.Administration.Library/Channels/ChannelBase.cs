using System;
using System.Net.Http;
using Maze.Modules.Api;

namespace Maze.Administration.Library.Channels
{
    /// <summary>
    ///     A base class for <see cref="IAwareDataChannel"/>
    /// </summary>
    public abstract class ChannelBase : IAwareDataChannel
    {
        /// <summary>
        ///     The required offset when sending data using <see cref="Send"/> with hasOffset: <code>true</code>
        /// </summary>
        protected int RequiredOffset;

        /// <summary>
        ///     Send data to the remote channel
        /// </summary>
        protected SendDelegate Send;

        /// <summary>
        ///     True if the current channel is disposed
        /// </summary>
        protected bool IsDisposed;

        private readonly object _disposeLock = new object();

        public void Dispose()
        {
            if (!IsDisposed)
            {
                bool dispose = false;
                lock (_disposeLock)
                {
                    if (!IsDisposed)
                    {
                        IsDisposed = true;
                        dispose = true;
                    }
                }

                if (dispose)
                {
                    CloseChannel?.Invoke(this, EventArgs.Empty);
                    InternalDispose();
                }
            }
        }

        /// <summary>
        ///     Dispose resources. It is guaranteed that this method will only be called once.
        /// </summary>
        protected virtual void InternalDispose()
        {
        }

        event EventHandler IAwareDataChannel.CloseChannel
        {
            add => CloseChannel += value;
            remove => CloseChannel -= value;
        }

        int IDataChannel.RequiredOffset
        {
            get => RequiredOffset;
            set => RequiredOffset = value;
        }

        SendDelegate IDataChannel.Send
        {
            get => Send;
            set => Send = value;
        }

        void IAwareDataChannel.Initialize(HttpResponseMessage responseMessage)
        {
            Initialize(responseMessage);
        }

        void IDataChannel.ReceiveData(byte[] buffer, int offset, int count)
        {
            if (IsDisposed)
                return;

            ReceiveData(buffer, offset, count);
        }

        private event EventHandler CloseChannel;

        /// <summary>
        ///     This method is called when data from the remote channel was received
        /// </summary>
        /// <param name="buffer">TThe data buffer</param>
        /// <param name="offset">The offset in the <see cref="buffer"/> where the data begins</param>
        /// <param name="count">The size of data found in the <see cref="buffer"/></param>
        protected abstract void ReceiveData(byte[] buffer, int offset, int count);

        /// <summary>
        ///     Initialize the data channel after it was activated
        /// </summary>
        /// <param name="responseMessage">The response message that created this channel</param>
        protected virtual void Initialize(HttpResponseMessage responseMessage)
        {
        }
    }
}