using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;

namespace Orcus.Server.Service.Commander
{
    public class HttpOrcusContextWrapper : OrcusContext
    {
        private readonly HttpContext _httpContext;

        public HttpOrcusContextWrapper(HttpContext httpContext)
        {
            _httpContext = httpContext;
            Request = new HttpOrcusRequestWrapper(httpContext.Request);
            Response = new HttpOrcusResponseWrapper(httpContext.Response);
        }

        public override object Caller { get; set; }
        public override OrcusRequest Request { get; set; }
        public override OrcusResponse Response { get; set; }
        public override ConnectionInfo Connection { get; set; }

        public override IServiceProvider RequestServices
        {
            get => _httpContext.RequestServices;
            set => _httpContext.RequestServices = value;
        }

        public override CancellationToken RequestAborted
        {
            get => _httpContext.RequestAborted;
            set => _httpContext.RequestAborted = value;
        }

        public override void Abort()
        {
            _httpContext.Abort();
        }
    }
}
