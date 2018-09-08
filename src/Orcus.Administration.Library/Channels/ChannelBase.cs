using System;
using System.Net.Http;
using Orcus.Modules.Api;

namespace Orcus.Administration.Library.Channels
{
    public abstract class ChannelBase : IAwareDataChannel
    {
        protected int RequiredOffset;
        protected SendDelegate Send;

        public virtual void Dispose()
        {
            CloseChannel?.Invoke(this, EventArgs.Empty);
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