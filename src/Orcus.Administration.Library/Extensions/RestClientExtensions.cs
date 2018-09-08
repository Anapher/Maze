using NuGet.Packaging.Core;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Models;

namespace Orcus.Administration.Library.Extensions
{
    public static class RestClientExtensions
    {
        public static ITargetedRestClient CreateTargeted(this IOrcusRestClient orcusRestClient, int clientId)
        {
            return new TargetedRestClient(orcusRestClient, clientId);
        }

        public static ITargetedRestClient CreateTargeted(this IOrcusRestClient orcusRestClient, ClientViewModel client)
        {
            return CreateTargeted(orcusRestClient, client.ClientId);
        }

        public static IPackageRestClient CreatePackageSpecific(this ITargetedRestClient targetedRestClient, string packageName)
        {
            return new PackageRestClient(targetedRestClient, packageName);
        }

        public static IPackageRestClient CreatePackageSpecific(this ITargetedRestClient targetedRestClient, PackageIdentity packageId)
        {
            return new PackageRestClient(targetedRestClient, packageId.Id);
        }
    }
}