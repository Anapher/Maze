using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Server.Library.Services;
using Orcus.Server.OrcusSockets;
using Orcus.Server.OrcusSockets.Internal;
using Orcus.Server.Service.Commander;
using Orcus.Server.Service.Extensions;
using Orcus.Service.Commander;

//using Orcus.Server.Service.Modules.Execution;

namespace Orcus.Server.Service
{
    public interface ICommandDistributer
    {
        Task Execute(OrcusRequest request, CommandTargetCollection targets, CommandExecutionPolicy executionPolicy);
        Task<HttpResponseMessage> Execute(HttpRequestMessage request, CommandTarget target);
    }

    public class CommandDistributer : ICommandDistributer
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IOrcusRequestExecuter _localRequestExecuter;

        public CommandDistributer(IOrcusRequestExecuter localRequestExecuter, IConnectionManager connectionManager)
        {
            _localRequestExecuter = localRequestExecuter;
            _connectionManager = connectionManager;
        }

        public Task Execute(OrcusRequest request, CommandTargetCollection targets,
            CommandExecutionPolicy executionPolicy) =>
            throw new NotImplementedException();

        public async Task<HttpResponseMessage> Execute(HttpRequestMessage request, CommandTarget target)
        {
            var orcusRequest = request.ToOrcusRequest();
            await orcusRequest.InitializeBody();

            //var orcusResponse = new DefaultOrcusResponse(1);
            //orcusResponse.Body = new PackagingBufferStream(data => );

            //var context = new DefaultOrcusContext(orcusRequest, new DefaultOrcusResponse(1), null );
            //await _localRequestExecuter.Execute(context);


            //if (_connectionManager.ClientConnections.TryGetValue(target.Id, out var clientConnection))
            //    return await clientConnection.OrcusServer.SendRequest(request);

            throw new InvalidOperationException("Client not found"); //TODO: response?
        }
    }
}