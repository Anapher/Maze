using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;
using Orcus.Server.Service.Modules.Routing;

namespace Orcus.Server.Service.Commanding
{
    public class ActionContext : IActionContext
    {
        public Route Route { get; set; }
        public OrcusContext OrcusContext { get; set; }
        public IDictionary<string, object> RouteValues { get; set; }

        public IServiceProvider ServiceProvider { get; }
        public OrcusResponse Response { get; }
    }

    public class Cody : ICommander
    {
        public Cody()
        {
            
        }

        public Task<OrcusResponse> MakeRequest(OrcusRequest request, OrcusRequestTarget target)
        {
            if (target.IsServer)
                return ExecuteServerRequest(request);

            throw new NotImplementedException();
        }

        public Task<OrcusResponse> ExecuteServerRequest(OrcusRequest request)
        {

        }
    }

    public interface ICommander
    {
        Task<OrcusResponse> MakeRequest(OrcusRequest request, OrcusRequestTarget target);
    }

    public class OrcusRequestMetadata
    {
        public string Target { get; set; }
    }

    public class OrcusRequestTarget
    {
        public bool IsServer { get; set; }
    }
}