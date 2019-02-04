using System;
using System.Buffers;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Maze.Administration.Core.Extensions;
using Maze.Administration.Library.Channels;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Exceptions;
using Maze.Modules.Api;
using Maze.Server.Connection;
using Maze.Server.Connection.Authentication;
using Maze.Server.Connection.Error;
using Maze.Sockets;
using Maze.Sockets.Client;
using Maze.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Maze.Administration.Core.Rest
{
    public class MazeRestClient : IMazeRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly SecureString _password;
        private JwtSecurityToken _jwtSecurityToken;
        private MazeServer _mazeServer;
        private readonly SemaphoreSlim _mazeServerLock = new SemaphoreSlim(1, 1);

        public MazeRestClient(string username, IServerInfo serverInfo, SecureString password, HttpClient httpClient)
        {
            _password = password;
            Server = serverInfo;
            _httpClient = httpClient;
            Username = username;
            _jwtHandler = new JwtSecurityTokenHandler();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _password.Dispose();
            _mazeServer?.Dispose();
            _mazeServer = null;
        }

        public string Username { get; private set; }
        public IServerInfo Server { get; private set; }
        public HubConnection HubConnection { get; private set; }
        public IServiceProvider ServiceProvider { get; set; }

        public async Task<TChannel> OpenChannel<TChannel>(HttpRequestMessage message, CancellationToken cancellationToken)
            where TChannel : IAwareDataChannel
        {
            var connection = await GetServerConnection();

            var response = await SendMessage(message, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new InvalidOperationException("The channel was not created");

            var channelId = int.Parse(response.Headers.Location.AbsolutePath.Trim('/'));
            var channel = ServiceProvider.GetRequiredService<TChannel>();
            channel.CloseChannel += (sender, args) => OnCloseChannel(channelId);
            connection.AddChannel(channel, channelId);

            channel.Initialize(response);

            return channel;
        }

        public Task<HttpResponseMessage> SendChannelMessage(HttpRequestMessage request, IDataChannel channel, CancellationToken cancellationToken)
        {
            if (_mazeServer == null)
                throw new InvalidOperationException("The channel is not open");

            var id = _mazeServer.GetChannelId(channel);
            request.Headers.Add("ChannelId", id.ToString());
            return SendMessage(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> SendMessage(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (response.IsSuccessStatusCode)
                return response;

            var result = await response.Content.ReadAsStringAsync();

            RestError[] errors = null;
            if (!string.IsNullOrEmpty(result))
            {
                if (result[0] == '[')
                    try
                    {
                        errors = JsonConvert.DeserializeObject<RestError[]>(result);
                    }
                    catch (Exception)
                    {
                        throw new HttpRequestException(result);
                    }
                else
                    try
                    {
                        errors = new[] {JsonConvert.DeserializeObject<RestError>(result)};
                    }
                    catch (Exception)
                    {
                        throw new HttpRequestException(result);
                    }
            }

            if (errors == null)
                response.EnsureSuccessStatusCode();

            var error = errors[0];
            switch (error.Type)
            {
                case ErrorTypes.ValidationError:
                    throw new RestArgumentException(error);
                case ErrorTypes.AuthenticationError:
                    throw new RestAuthenticationException(error);
                case ErrorTypes.NotFoundError:
                    throw new RestNotFoundException(error);
                case ErrorTypes.InvalidOperationError:
                    throw new RestInvalidOperationException(error);
            }

            Debug.Fail($"The error type {error.Type} is not implemented.");
            throw new NotSupportedException(error.Message);
        }

        private void ConfigureHttpConnection(HttpConnectionOptions obj)
        {
            obj.Headers.Add(HeaderNames.Authorization, _httpClient.DefaultRequestHeaders.Authorization.ToString());
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

                using (var response = await SendMessage(request, CancellationToken.None))
                {
                    var authorizationToken = await response.Content.ReadAsStringAsync();
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authorizationToken);

                    _jwtSecurityToken = _jwtHandler.ReadJwtToken(authorizationToken);
                    Username = _jwtSecurityToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.GivenName)
                        .Value; //because the username is case insensitive
                }

                HubConnection = new HubConnectionBuilder()
                    .WithUrl(new Uri(_httpClient.BaseAddress, "signalR"), ConfigureHttpConnection).Build();

                await HubConnection.StartAsync();
            }
        }

        private void OnCloseChannel(int channelId)
        {
            if (_mazeServer != null)
                try
                {
                    _mazeServer?.CloseChannel(channelId);
                }
                catch (Exception)
                {
                    _mazeServer.Dispose();
                    _mazeServer = null;
                }
        }

        protected async Task<MazeServer> GetServerConnection()
        {
            if (_mazeServer != null)
                return _mazeServer;

            await _mazeServerLock.WaitAsync();
            try
            {
                if (_mazeServer != null)
                    return _mazeServer;

                var builder = new UriBuilder(_httpClient.BaseAddress) { Path = "ws", Scheme = _httpClient.BaseAddress.Scheme == "https" ? "wss" : "ws" };

                var connector = new MazeSocketConnector(builder.Uri) { AuthenticationHeaderValue = _httpClient.DefaultRequestHeaders.Authorization };
                var dataStream = await connector.ConnectAsync();
                var webSocket = WebSocket.CreateClientWebSocket(dataStream, null, 8192, 8192, TimeSpan.FromMinutes(2), true,
                    WebSocket.CreateClientBuffer(8192, 8192));

                var webSocketWrapper = new WebSocketWrapper(webSocket, 8192);
                _mazeServer = new MazeServer(webSocketWrapper, 8192, 4096, ArrayPool<byte>.Shared);

                webSocketWrapper.ReceiveAsync().ContinueWith(ReceiveAsyncContinuation).Forget();
                return _mazeServer;
            }
            finally
            {
                _mazeServerLock.Release();
            }
        }

        private void ReceiveAsyncContinuation(Task obj)
        {
            _mazeServer?.Dispose();
            _mazeServer = null;
        }
    }
}