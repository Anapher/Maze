using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeElements.NetworkCall;
using CodeElements.NetworkCall.Extensions;
using CodeElements.NetworkCall.NetSerializer;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Extensions;

namespace Orcus.ControllerExtensions
{
    public abstract class CallTransmissionChannel<TInterface> : OrcusChannel
    {
        protected readonly Dictionary<string, Func<IServiceProvider, INetworkSerializer>> Serializers =
            new Dictionary<string, Func<IServiceProvider, INetworkSerializer>>(StringComparer.OrdinalIgnoreCase)
            {
                {"netserializer", provider => provider.GetRequiredService<NetSerializerNetworkSerializer>()}
            };

        protected NetworkCallServer<TInterface> Server;

        protected CallTransmissionChannel()
        {
            if (!typeof(TInterface).IsInterface)
                throw new ArgumentException("The TInterface must be an interface.", nameof(TInterface));

            if (!typeof(TInterface).IsAssignableFrom(GetType()))
                throw new ArgumentException($"{GetType().FullName} must implement {typeof(TInterface)}");
        }

        public override void Initialize()
        {
            var requestHeaders = OrcusContext.Request.GetTypedHeaders();
            var serializer = GetSerializer(requestHeaders);

            var thisObj = (object) this;
            Server = new NetworkCallServer<TInterface>((TInterface) thisObj, serializer,
                NetworkCallServerCacheProvider<TInterface>.Cache) {CustomOffset = RequiredOffset, SendData = SendData};
        }

        protected virtual INetworkSerializer GetSerializer(RequestHeaders headers)
        {
            foreach (var acceptedEncoder in headers.AcceptEncoding)
            {
                if (Serializers.TryGetValue(acceptedEncoder.Value.ToString(), out var getSerializerAction))
                {
                    Response.Headers.Add(HeaderNames.ContentEncoding, acceptedEncoder.Value.ToString());
                    return getSerializerAction(OrcusContext.RequestServices);
                }
            }

            Response.Headers.Add(HeaderNames.ContentEncoding, "netserializer");
            return OrcusContext.RequestServices.GetRequiredService<NetSerializerNetworkSerializer>(); //default
        }

        private Task SendData(BufferSegment data) => Send(data.Buffer, data.Offset, data.Length, true);

        public override void ReceiveData(byte[] buffer, int offset, int count)
        {
            Server.ReceiveData(buffer, offset);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            Server.Dispose();
        }
    }
}