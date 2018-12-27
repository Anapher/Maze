using Orcus.Modules.Api;

namespace Orcus.Service.Commander.Routing
{
    public interface IRouteResolver
    {
        ResolveResult Resolve(OrcusContext context);
    }
}