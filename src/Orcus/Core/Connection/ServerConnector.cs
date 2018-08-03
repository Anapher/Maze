using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Orcus.Core.Rest.Authentication.V1;
using Orcus.Logging;
using Orcus.Server.Connection.Authentication.Client;

namespace Orcus.Core.Connection
{
    public class ServerConnector
    {
        private static readonly ILog Logger = LogProvider.For<ServerConnector>();

        private readonly HttpClient _httpClient;

        public ServerConnector(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task TryConnect()
        {
            AuthenticationResource.Authenticate(new ClientAuthenticationDto(), null);
        }
    }
}
