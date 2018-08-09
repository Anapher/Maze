using System;
using System.Threading.Tasks;

namespace Orcus.Sockets
{
    public delegate Task SendMessageDelegate(OrcusChannel channel, ArraySegment<byte> data);

    public abstract class OrcusChannel : IDisposable
    {
        public SendMessageDelegate SendMessage { get; set; }

        public abstract void InvokeMessage(ArraySegment<byte> data);

        public virtual void Dispose()
        {
        }
    }
}