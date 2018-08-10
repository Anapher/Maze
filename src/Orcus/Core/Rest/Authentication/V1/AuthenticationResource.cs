using System.Threading.Tasks;
using NuGet.Protocol;
using Orcus.Clients;
using Orcus.Clients.Helpers;
using Orcus.Server.Connection.Authentication.Client;
using Orcus.Server.Connection.JsonConverters;

namespace Orcus.Core.Rest.Authentication.V1
{
    public class AuthenticationResource : VersionedResource<AuthenticationResource>
    {
        public AuthenticationResource() : base("clients")
        {
        }

        public static Task<ClientAuthenticationResponse> Authenticate(ClientAuthenticationDto dto, IRestClient client)
        {
            return CreateRequest(HttpVerb.Post, "login", new JsonContent(dto, settings =>
            {
                settings.Converters.Add(new NuGetFrameworkConverter());
                settings.Converters.Add(new NuGetVersionConverter());
            })).Execute(client).Wrap<ClientAuthenticationResponse>().ConfigureJsonSettings(settings =>
            {
                settings.Converters.Add(new PackageIdentityConverter());
            }).ToResult();
        }
    }
}