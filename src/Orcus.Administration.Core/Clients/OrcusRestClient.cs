using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Orcus.Administration.Core.Exceptions;
using Orcus.Administration.Core.Extensions;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Authentication;
using Orcus.Server.Connection.Error;

namespace Orcus.Administration.Core.Clients
{
    public class OrcusRestClient : IOrcusRestClient
    {
        private readonly SecureString _password;
        private readonly HttpClient _httpClient;
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private JwtSecurityToken _jwtSecurityToken;

        public OrcusRestClient(string username, SecureString password, HttpClient httpClient)
        {
            _password = password;
            _httpClient = httpClient;
            Username = username;
            _jwtHandler = new JwtSecurityTokenHandler();
        }

        public string Username { get; private set; }

        public async Task<HttpResponseMessage> SendMessage(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
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

        public async Task<HubConnection> CreateHubConnection(string resource)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(new Uri(_httpClient.BaseAddress,
                    resource + "?signalRTokenHeader=" + _httpClient.DefaultRequestHeaders.Authorization.Parameter))
                .Build();

            await connection.StartAsync();
            return connection;
        }

        public async Task Initialize()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "v1/accounts/login"))
            {
                using (var secureStringWrapper = new SecureStringWrapper(_password))
                {
                    request.Content = new JsonContent(new LoginInfo
                    {
                        Username = Username,
                        Password = Encoding.UTF8.GetString(secureStringWrapper.ToByteArray())
                    });
                }

                using (var response = await SendMessage(request))
                {
                    var authorizationToken = await response.Content.ReadAsStringAsync();
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authorizationToken);

                    _jwtSecurityToken = _jwtHandler.ReadJwtToken(authorizationToken);
                    Username = _jwtSecurityToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.GivenName)
                        .Value; //because the username is case insensitive
                }
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _password.Dispose();
        }
    }
}