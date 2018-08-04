using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orcus.Client.Library.Services;
using Orcus.Clients;
using Orcus.Exceptions;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Error;

namespace Orcus.Core.Connection
{
    public class OrcusRestClient : IRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;
        private string _jwt;

        public OrcusRestClient(HttpClient httpClient, Uri baseUri)
        {
            _httpClient = httpClient;
            _baseUri = baseUri;
        }

        public void SetAuthenticated(string jwt)
        {
            _jwt = jwt;
        }

        public async Task<HttpResponseMessage> SendMessage(HttpRequestMessage request)
        {
            if (!request.RequestUri.IsAbsoluteUri)
                request.RequestUri = new Uri(_baseUri, request.RequestUri);

            if (_jwt != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer ", _jwt);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return response;

            var result = await response.Content.ReadAsStringAsync();

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
    }
}
