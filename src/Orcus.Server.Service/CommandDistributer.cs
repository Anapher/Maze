using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;
using Orcus.Server.Service.Connection;
using Orcus.Server.Service.Extensions;
using Orcus.Server.Service.Modules.Execution;

namespace Orcus.Server.Service
{
    public interface ICommandDistributer
    {
        Task Execute(OrcusRequest request, CommandTargetCollection targets,
            CommandExecutionPolicy executionPolicy);

        Task<HttpResponseMessage> Execute(HttpRequestMessage request, CommandTarget target);
    }

    public class CommandDistributer : ICommandDistributer
    {
        private readonly IOrcusRequestExecuter _localRequestExecuter;
        private readonly IConnectionManager _connectionManager;

        public CommandDistributer(IOrcusRequestExecuter localRequestExecuter, IConnectionManager connectionManager)
        {
            _localRequestExecuter = localRequestExecuter;
            _connectionManager = connectionManager;
        }

        public Task Execute(OrcusRequest request, CommandTargetCollection targets,
            CommandExecutionPolicy executionPolicy)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> Execute(HttpRequestMessage request, CommandTarget target)
        {
            if (target.Type == CommandTargetType.Server)
                return (await _localRequestExecuter.Execute(request.ToOrcusRequest())).ToHttpResponseMessage();

            if (_connectionManager.ClientConnections.TryGetValue(target.Id, out var clientConnection))
                return await clientConnection.OrcusServer.SendRequest(request);

            throw new InvalidOperationException("Client not found"); //TODO: response?
        }
    }
}