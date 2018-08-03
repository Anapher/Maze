using System.Threading.Tasks;
using Orcus.Clients;
using Orcus.Clients.Helpers;
using Orcus.Server.Connection.Authentication.Client;

namespace Orcus.Core.Rest.Authentication.V1
{
    public class AuthenticationResource : VersionedResource<AuthenticationResource>
    {
        public AuthenticationResource() : base("clients")
        {
        }

        public static Task<ClientAuthenticationResponse> Authenticate(ClientAuthenticationDto dto, IRestClient client)
        {
            return CreateRequest(HttpVerb.Post).Execute(client).Return<ClientAuthenticationResponse>();
        }
    }
}