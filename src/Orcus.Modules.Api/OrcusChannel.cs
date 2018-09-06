using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Modules.Api
{
    public abstract class OrcusChannel : OrcusController
    {
        public int ChannelId { get; set; }
        public SendDelegate SendDelegate { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public abstract Task ReceiveData(byte[] buffer, int offset, int count);

        public virtual void Initialize()
        {
        }
    }

    public abstract class OrcusEventChannel : OrcusChannel
    {
        public override Task ReceiveData(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    public delegate Task SendDelegate(byte[] buffer, int offset, int count);
}