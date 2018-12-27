using Orcus.Server.Connection.Modules;

namespace Orcus.Server.Connection.Authentication.Client
{
    public class ClientAuthenticationResponse
    {
        public string Jwt { get; set; }
        public PackagesLock Modules { get; set; }
    }
}