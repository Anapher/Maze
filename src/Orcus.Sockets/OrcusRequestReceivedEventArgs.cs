using System;
using System.Threading;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;

namespace Orcus.Sockets
{
    public class OrcusRequestReceivedEventArgs : EventArgs
    {
        public OrcusRequestReceivedEventArgs(OrcusRequest request, OrcusResponse response,
            CancellationToken cancellationToken)
        {
            Request = request;
            Response = response;
            CancellationToken = cancellationToken;
        }

        public OrcusRequest Request { get; }
        public OrcusResponse Response { get; }
        public CancellationToken CancellationToken { get; }
    }
}