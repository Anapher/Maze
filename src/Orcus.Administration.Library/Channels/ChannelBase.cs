using System;
using System.Net.Http;
using Orcus.Modules.Api;

namespace Orcus.Administration.Library.Channels
{
    public abstract class ChannelBase : IAwareDataChannel
    {
        protected int RequiredOffset;
        protected SendDelegate Send;
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
            ReceiveData(buffer, offset, count);
        }

        private event EventHandler CloseChannel;

        protected abstract void ReceiveData(byte[] buffer, int offset, int count);

        protected virtual void Initialize(HttpResponseMessage responseMessage)
        {
        }
    }
}