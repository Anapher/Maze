using System;
using System.Threading.Tasks;
using CodeElements.NetworkCall;
using CodeElements.NetworkCall.Extensions;
using Orcus.Modules.Api;

namespace Orcus.ControllerExtensions
{
    public abstract class CallTransmissionChannel<TInterface> : OrcusChannel
    {
        protected readonly NetworkCallServer<TInterface> Server;

        protected CallTransmissionChannel(INetworkSerializer networkSerializer)
        {
            if (!typeof(TInterface).IsInterface)
                throw new ArgumentException("The TInterface must be an interface.", nameof(TInterface));

            var thisObj = (object) this;
            if (!typeof(TInterface).IsAssignableFrom(GetType()))
                throw new ArgumentException($"{GetType().FullName} must implement {typeof(TInterface)}");

            Server = new NetworkCallServer<TInterface>((TInterface) thisObj, networkSerializer,
                NetworkCallServerCacheProvider<TInterface>.Cache);
        }

        public override void Initialize()
        {
            Server.CustomOffset = RequiredOffset;
            Server.SendData = SendData;
        }

        private Task SendData(BufferSegment data) => Send(data.Buffer, data.Offset, data.Length, true);

        public override void ReceiveData(byte[] buffer, int offset, int count)
        {
            Server.ReceiveData(buffer, offset);
        }
    }
}