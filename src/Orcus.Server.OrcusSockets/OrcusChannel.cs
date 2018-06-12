using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Orcus.Server.OrcusSockets
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