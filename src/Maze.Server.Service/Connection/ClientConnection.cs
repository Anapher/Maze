using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Error;
using Orcus.Server.Library.Exceptions;
using Orcus.Server.Library.Services;
using Orcus.Sockets;

namespace Orcus.Server.Service.Connection
{
    public class ClientConnection : IClientConnection, IDisposable
    {
        public ClientConnection(int clientId, WebSocketWrapper webSocket, OrcusServer orcusServer)
        {
            ClientId = clientId;
            WebSocketWrapper = webSocket;
            OrcusServer = orcusServer;
        }

        public void Dispose()
        {
            WebSocketWrapper?.Dispose();
            OrcusServer?.Dispose();
        }

        public int ClientId { get; }
        public WebSocketWrapper WebSocketWrapper { get; }
        public OrcusServer OrcusServer { get; }

        public Task BeginListen()
        {
            return WebSocketWrapper.ReceiveAsync();
        }

        public async Task<HttpResponseMessage> SendMessage(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            var response = await OrcusServer.SendRequest(requestMessage, cancellationToken);
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
    }
}