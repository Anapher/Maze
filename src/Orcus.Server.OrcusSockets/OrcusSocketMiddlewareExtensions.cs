using Microsoft.AspNetCore.Builder;

namespace Orcus.Server.OrcusSockets
{
    /// <summary>
    ///     Extensions of the <see cref="IApplicationBuilder" /> to enqueue the <see cref="OrcusSocketMiddleware" />
    /// </summary>
    public static class OrcusSocketMiddlewareExtensions
    {
        /// <summary>
        ///     Enqueue the <see cref="OrcusSocketMiddleware" /> and provide the <see cref="IOrcusSocketFeature" /> in the context
        ///     if possible
        /// </summary>
        /// <param name="builder">The application builder of your server</param>
        public static IApplicationBuilder UseOrcusSockets(this IApplicationBuilder builder) =>
            builder.UseMiddleware<OrcusSocketMiddleware>();
    }
}