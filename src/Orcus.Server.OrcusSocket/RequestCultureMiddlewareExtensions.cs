using Microsoft.AspNetCore.Builder;

namespace Orcus.Server.OrcusSockets
{
    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseOrcusSockets(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OrcusSocketMiddleware>();
        }
    }
}