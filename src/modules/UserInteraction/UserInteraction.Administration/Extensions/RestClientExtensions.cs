using Orcus.Administration.Library.Clients;

namespace UserInteraction.Administration.Extensions
{
    public static class RestClientExtensions
    {
        public static IPackageRestClient CreateLocal(this ITargetedRestClient restClient) =>
            restClient.CreatePackageSpecific("UserInteraction");
    }
}