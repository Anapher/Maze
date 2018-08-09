using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;
using Orcus.Sockets;

namespace Orcus.Core.Commanding
{
    public class WebSocketOrcusContext : OrcusContext
    {
        public WebSocketOrcusContext(OrcusRequestReceivedEventArgs args)
        {
            Request = args.Request;
            Response = args.Response;

            Request.Context = this;
        }

        public override object Caller { get; set; }
        public override OrcusRequest Request { get; set; }
        public override OrcusResponse Response { get; set; }
        public override ConnectionInfo Connection { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}