using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;

namespace Orcus.Service.Commander
{
    public class DefaultOrcusContext : OrcusContext
    {
        private readonly CancellationTokenSource _requestCancellationTokenSource;

        public DefaultOrcusContext(OrcusRequest request, IServiceProvider serviceProvider)
        {
            Request = request;
            RequestServices = serviceProvider;

            _requestCancellationTokenSource = new CancellationTokenSource();
            RequestAborted = _requestCancellationTokenSource.Token;
        }

        public override OrcusResponse Response { get; set; }
        public override object Caller { get; set; }
        public override OrcusRequest Request { get; set; }
        public override ConnectionInfo Connection { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }

        public override void Abort()
        {
            _requestCancellationTokenSource.Cancel();
        }
    }
}