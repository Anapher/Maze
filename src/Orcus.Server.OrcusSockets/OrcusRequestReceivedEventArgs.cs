using System;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;

namespace Orcus.Server.OrcusSockets
{
    public class OrcusRequestReceivedEventArgs : EventArgs
    {
        public OrcusRequestReceivedEventArgs(OrcusRequest request, OrcusResponse response)
        {
            Request = request;
            Response = response;
        }

        public OrcusRequest Request { get; }
        public OrcusResponse Response { get; }
    }
}