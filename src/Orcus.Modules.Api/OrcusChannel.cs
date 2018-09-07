using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Modules.Api
{
    public abstract class OrcusChannel : OrcusController, IDataChannel
    {
        public int ChannelId { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public int RequiredOffset { get; set; }

        public SendDelegate Send { get; set; }

        public abstract void ReceiveData(byte[] buffer, int offset, int count);

        public virtual void Initialize()
        {
        }
    }

    public delegate Task SendDelegate(byte[] buffer, int offset, int count, bool hasOffset);
}