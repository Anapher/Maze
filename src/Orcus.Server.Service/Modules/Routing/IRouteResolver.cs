using Orcus.Modules.Api;

namespace Orcus.Server.Service.Modules.Routing
{
    public interface IRouteResolver
    {
        ResolveResult Resolve(OrcusContext context);
    }
}