using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;
using Orcus.Service.Commander.Routing;

namespace Orcus.Service.Commander
{
    public class DefaultActionContext : ActionContext
    {
        public DefaultActionContext(OrcusContext context, Route route, IReadOnlyDictionary<string, object> routeData)
        {
            Context = context;
            Route = route;
            RouteData = routeData;
        }

        public override OrcusContext Context { get; }
        public Route Route { get; }
        public override IReadOnlyDictionary<string, object> RouteData { get; }
    }

    ///// <summary>
    /////     The commander
    ///// </summary>
    //public class Cody : ICommander
    //{
    //    private readonly IOrcusRequestExecuter _requestExecuter;

    //    public Cody(IOrcusRequestExecuter requestExecuter)
    //    {
    //        _requestExecuter = requestExecuter;
    //    }

    //    public Task<OrcusResponse> MakeRequest(OrcusRequest request, OrcusRequestTarget target)
    //    {
    //        if (target.IsServer)
    //            return _requestExecuter.Execute(request);

    //        throw new NotImplementedException();
    //    }

    //    public Task<OrcusResponse> ExecuteServerRequest(OrcusRequest request)
    //    {
    //        return _requestExecuter.Execute(request);
    //    }
    //}

    //public interface ICommander
    //{
    //    Task<OrcusResponse> MakeRequest(OrcusRequest request, OrcusRequestTarget target);
    //}

    //public class OrcusRequestMetadata
    //{
    //    public string Target { get; set; }
    //}

    //public class OrcusRequestTarget
    //{
    //    public bool IsServer { get; set; }
    //}
}