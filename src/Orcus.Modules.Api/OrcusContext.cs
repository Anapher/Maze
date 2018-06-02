using System;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;

namespace Orcus.Modules.Api
{
    public abstract class OrcusContext
    {
        public abstract OrcusResponse Response { get; set; }
        public abstract object Caller { get; set; }
        public abstract OrcusRequest Request { get; set; }
        public abstract ConnectionInfo Connection { get; set; }
        public abstract IServiceProvider ServiceProvider { get; set; }
    }
}