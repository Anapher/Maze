using Microsoft.Extensions.DependencyInjection;

namespace Orcus.Service.Commander
{
    public static class OrcusServerServices
    {
        public static IServiceCollection RegisterOrcusServices(this IServiceCollection serviceCollection) =>
            serviceCollection;
    }
}