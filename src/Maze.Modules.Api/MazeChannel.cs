using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Modules.Api
{
    public abstract class OrcusChannel : OrcusController, IDataChannel
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        protected OrcusChannel()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;
        }

        public int ChannelId { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public int RequiredOffset { get; set; }

        public SendDelegate Send { get; set; }

        public abstract void ReceiveData(byte[] buffer, int offset, int count);

        public virtual void Initialize()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            _cancellationTokenSource.Cancel();
        }
    }

    public delegate Task SendDelegate(byte[] buffer, int offset, int count, bool hasOffset);
}