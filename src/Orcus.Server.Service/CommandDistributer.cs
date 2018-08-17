using System;
using System.Net.Http;
using System.Threading.Tasks;
using Orcus.Modules.Api.Request;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Library.Services;

namespace Orcus.Server.Service
{
    public interface ICommandDistributer
    {
        Task Execute(OrcusRequest request, CommandTargetCollection targets, CommandExecutionPolicy executionPolicy);
        Task<HttpResponseMessage> Execute(HttpRequestMessage request, int clientId);
    }

    public class CommandDistributer : ICommandDistributer
    {
        private readonly IConnectionManager _connectionManager;

        public CommandDistributer(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public Task Execute(OrcusRequest request, CommandTargetCollection targets,
            CommandExecutionPolicy executionPolicy) =>
            throw new NotImplementedException();

        public Task<HttpResponseMessage> Execute(HttpRequestMessage request, int clientId)
        {
            if (_connectionManager.ClientConnections.TryGetValue(clientId, out var clientConnection))
                return clientConnection.SendRequest(request);

            throw new ClientNotFoundException();
        }
    }

    public class ClientNotFoundException : Exception
    {
    }
}