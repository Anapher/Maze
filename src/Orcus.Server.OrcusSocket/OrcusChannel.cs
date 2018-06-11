using System;

namespace Orcus.Server.OrcusSockets
{
    public abstract class OrcusChannel : IDisposable
    {
        public event EventHandler<ArraySegment<byte>> SendMessage;

        public abstract void InvokeMessage(OrcusMessage message);

        public virtual void Dispose()
        {
        }
    }
}