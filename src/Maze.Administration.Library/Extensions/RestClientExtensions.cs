using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Models;

namespace Orcus.Administration.Library.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="IOrcusRestClient"/>
    /// </summary>
    public static class RestClientExtensions
    {
        /// <summary>
        ///     Create a <see cref="ITargetedRestClient"/> based on a <see cref="clientId"/>
        /// </summary>
        /// <param name="orcusRestClient">The rest client that carries the core connection.</param>
        /// <param name="clientId">The id of the client that should be called by the new rest client.</param>
        /// <returns>Return the new rest client that will send all commands to the client.</returns>
        public static ITargetedRestClient CreateTargeted(this IOrcusRestClient orcusRestClient, int clientId)
        {
            return new TargetedRestClient(orcusRestClient, clientId);
        }

        /// <summary>
        ///     Create a <see cref="ITargetedRestClient"/> based on a <see cref="client"/>
        /// </summary>
        /// <param name="orcusRestClient">The rest client that carries the core connection.</param>
        /// <param name="client">The client that should be called by the new rest client.</param>
        /// <returns>Return the new rest client that will send all commands to the client.</returns>
        public static ITargetedRestClient CreateTargeted(this IOrcusRestClient orcusRestClient, ClientViewModel client)
        {
            return CreateTargeted(orcusRestClient, client.ClientId);
        }
    }
}