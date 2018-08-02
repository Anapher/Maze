using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Logging;
using Orcus.Server.Connection.Authentication.Client;

namespace Orcus.Connection
{
    public class ServerConnector
    {
        private static HttpClient _httpClient;
        private static readonly object HttpClientLock = new object();
        private static readonly ILog Logger = LogProvider.For<ServerConnector>();
        private readonly ClientAuthenticationDto _clientAuthentication;
        private readonly IConnectionSettings _connectionSettings;

        public ServerConnector(IConnectionSettings connectionSettings, ClientAuthenticationDto clientAuthentication)
        {
            _connectionSettings = connectionSettings;
            _clientAuthentication = clientAuthentication;
        }

        public async Task<ServerConnection> TryConnect(Uri serverUri)
        {
            var client = GetHttpClient();

            Logger.Info("Try establishing connection to {0}", serverUri);

            var serverConnection = new ServerConnection(client, serverUri);
            var timeoutSource = new CancellationTokenSource(_connectionSettings.ConnectionTimout);

            try
            {
                await serverConnection.Initialize(_clientAuthentication, timeoutSource.Token);
            }
            catch (TimeoutException)
            {
                Logger.Warn("The connection to server {0} timed out.", serverUri);
            }
            catch (Exception e)
            {
                Logger.WarnException("The connection to server {0} failed.", e, serverUri);
            }

            return serverConnection;
        }

        private HttpClient GetHttpClient()
        {
            if (_httpClient != null)
                return _httpClient;

            lock (HttpClientLock)
            {
                return _httpClient ?? (_httpClient = new HttpClient(_connectionSettings.GetMessageHandler(), true));
            }
        }
    }

    public interface IConnectionSettings
    {
        TimeSpan ConnectionTimout { get; }

        HttpMessageHandler GetMessageHandler();
    }
}