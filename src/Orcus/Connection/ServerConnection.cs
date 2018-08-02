using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orcus.Clients;
using Orcus.Connection.Exceptions;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Authentication.Client;
using Orcus.Server.Connection.Error;
using Orcus.Server.Connection.Modules;

namespace Orcus.Connection
{
    public class ServerConnection : IRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _serverUri;
        private AuthenticationHeaderValue _authenticationHeader;

        public ServerConnection(HttpClient httpClient, Uri serverUri)
        {
            _httpClient = httpClient;
            _serverUri = serverUri;
        }

        public PackagesLock Packages { get; private set; }

        public async Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.RequestUri.IsAbsoluteUri)
                request.RequestUri = new Uri(_serverUri, request.RequestUri);

            request.Headers.Authorization = _authenticationHeader;

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return response;

            RestError[] errors;
            try
            {
                errors = JsonConvert.DeserializeObject<RestError[]>(result);
            }
            catch (Exception)
            {
                throw new HttpRequestException(result);
            }

            if (errors == null)
                response.EnsureSuccessStatusCode();

            var error = errors[0];
            switch (error.Type)
            {
                case ErrorTypes.ValidationError:
                case ErrorTypes.AuthenticationError:
                    throw new RestAuthenticationException(error);
                case ErrorTypes.NotFoundError:
                    throw new RestNotFoundException(error);
            }

            Debug.Fail($"The error type {error.Type} is not implemented.");
            throw new NotSupportedException(error.Message);
        }

        public async Task Initialize(ClientAuthenticationDto clientAuthentication, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "v1/clients/login"))
            {
                request.Content = new JsonContent(clientAuthentication);

                using (var response = await SendMessage(request, cancellationToken))
                {
                    var data = JsonConvert.DeserializeObject<ClientAuthenticationResponse>(
                        await response.Content.ReadAsStringAsync());

                    _authenticationHeader = new AuthenticationHeaderValue("Bearer", data.Jwt);
                    Packages = data.Modules;
                }
            }
        }

        public Task<HttpResponseMessage> SendMessage(HttpRequestMessage request) =>
            SendMessage(request, CancellationToken.None);
    }
}
