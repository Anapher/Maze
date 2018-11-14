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
    }
}