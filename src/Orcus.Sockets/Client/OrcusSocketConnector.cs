using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Orcus.Sockets.Client.WebSocketSharp;
using Orcus.Sockets.Internal;

namespace Orcus.Sockets.Client
{
    public class OrcusSocketConnector
    {
        private readonly Uri _serverUri;
        private readonly TcpClient _tcpClient;
        private readonly string _base64Key;

        public OrcusSocketConnector(Uri serverUri)
        {
            _serverUri = serverUri;
            _tcpClient = new TcpClient();

            SslConfig = new ClientSslConfiguration(serverUri.DnsSafeHost);
            _base64Key = CreateBase64Key();
        }

        public ClientSslConfiguration SslConfig { get; set; }
        public AuthenticationHeaderValue AuthenticationHeaderValue { get; set; }

        public async Task<OrcusSocket> ConnectAsync(TimeSpan? keepAliveInterval)
        {
            var stream = await GetClientStream();
            await SendHandshakeRequest(stream);

            return new OrcusSocket(stream, keepAliveInterval);
        }

        private async Task SendHandshakeRequest(Stream stream)
        {
            var request = CreateHandshakeRequest();
            var response = await SendHttpMessage(stream, request);
            if (response.StatusCode != HttpStatusCode.SwitchingProtocols)
                throw new WebSocketException(CloseStatusCode.ServerError);
        }

        private static async Task<HttpResponseMessage> SendHttpMessage(Stream stream, HttpRequestMessage message)
        {
            var httpString = await message.GetHttpString();
            var buffer = Encoding.UTF8.GetBytes(httpString);
            stream.Write(buffer, 0, buffer.Length);

            return await stream.DeserializeResponse();
        }

        private HttpRequestMessage CreateHandshakeRequest()
        {
            var message = HttpRequest.CreateHandshakeRequest(_serverUri);

            message.Headers.Add(OrcusSocketHeaders.SecWebSocketKey, _base64Key);
            message.Headers.Add(OrcusSocketHeaders.SecWebSocketVersion, OrcusSocketHeaders.SupportedVersion);

            if (AuthenticationHeaderValue != null)
                message.Headers.Authorization = AuthenticationHeaderValue;

            return message;
        }

        private async Task<Stream> GetClientStream()
        {
            await _tcpClient.ConnectAsync(_serverUri.DnsSafeHost, _serverUri.Port);

            Stream stream = _tcpClient.GetStream();

            if (_serverUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                var conf = SslConfig;
                var host = conf.TargetHost;
                if (host != _serverUri.DnsSafeHost)
                    throw new WebSocketException(
                        CloseStatusCode.TlsHandshakeFailure, "An invalid host name is specified.");

                try
                {
                    var sslStream = new SslStream(
                        stream,
                        false,
                        conf.ServerCertificateValidationCallback,
                        conf.ClientCertificateSelectionCallback);

                    await sslStream.AuthenticateAsClientAsync(
                        host,
                        conf.ClientCertificates,
                        conf.EnabledSslProtocols,
                        conf.CheckCertificateRevocation);

                    stream = sslStream;
                }
                catch (Exception ex)
                {
                    throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, ex);
                }
            }

            return stream;
        }

        private static string CreateBase64Key()
        {
            using (var randomNumber = RandomNumberGenerator.Create())
            {
                var src = new byte[16];
                randomNumber.GetBytes(src);

                return Convert.ToBase64String(src);
            }
        }

        public void Dispose()
        {
        }
    }
}
