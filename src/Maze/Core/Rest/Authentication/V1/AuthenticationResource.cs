using System.Threading.Tasks;
using NuGet.Protocol;
using Maze.Client.Library.Clients;
using Maze.Client.Library.Clients.Helpers;
using Maze.Server.Connection.Authentication.Client;
using Maze.Server.Connection.JsonConverters;

namespace Maze.Core.Rest.Authentication.V1
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