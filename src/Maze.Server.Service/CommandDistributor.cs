using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Modules.Api.Request;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Library.Services;

namespace Orcus.Server.Service
{
    public class CommandDistributor : ICommandDistributer
    {
        private readonly IConnectionManager _connectionManager;

        public CommandDistributor(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public Task Execute(OrcusRequest request, CommandTargetCollection targets,
            CommandExecutionPolicy executionPolicy) =>
            throw new NotImplementedException();

        public async Task<HttpResponseMessage> Execute(HttpRequestMessage request, int clientId, int accountId, CancellationToken cancellationToken)
        {
            if (_connectionManager.ClientConnections.TryGetValue(clientId, out var clientConnection))
            {
                var response = await clientConnection.SendMessage(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Created && response.Headers.Location?.Host == "channels")
                {
                    if (_connectionManager.AdministrationConnections.TryGetValue(accountId, out var administrationConnection))
                    {
                        var channelId = int.Parse(response.Headers.Location.AbsolutePath.Trim('/'));

                        clientConnection.OrcusServer.AddChannelRedirect(channelId, administrationConnection.OrcusServer.DataSocket);
                        administrationConnection.OrcusServer.AddChannelRedirect(channelId, clientConnection.OrcusServer.DataSocket);
                    }
                }
                return response;
            }

            throw new ClientNotFoundException(clientId);
        }
    }
}