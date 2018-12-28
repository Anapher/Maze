using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Maze.Server.Data.EfCode;
using Maze.Server.Library.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Server.Rest.V1;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Common.Server.Commands
{
    public class WakeOnLanCommandExecutor : LoggingTaskExecutor, ITaskExecutor<WakeOnLanCommandInfo>
    {
        private readonly AppDbContext _context;
        private readonly IConnectionManager _connectionManager;

        public WakeOnLanCommandExecutor(AppDbContext context, IConnectionManager connectionManager)
        {
            _context = context;
            _connectionManager = connectionManager;
        }

        public async Task<HttpResponseMessage> InvokeAsync(WakeOnLanCommandInfo commandInfo, TargetId targetId, TaskExecutionContext context, CancellationToken cancellationToken)
        {
            if (targetId.IsServer)
                return NotExecuted("The server cannot execute this command as it must be already running which renders this command useless.");

            if (_connectionManager.ClientConnections.ContainsKey(targetId.ClientId))
            {
                this.LogInformation("The client is already connected.");
                return Log(HttpStatusCode.OK);
            }

            var client = await _context.Clients.FindAsync(targetId.ClientId);
            var macAddress = PhysicalAddress.Parse(client.MacAddress);

            this.LogInformation("Send magic package from server to {address}", macAddress);
            context.ReportStatus("Send magic package from server...");

            WakeOnLan(macAddress);
            if (commandInfo.TryOverClient)
            {
                this.LogDebug("Querying clients that had the same ip address like the targeted client...");

                var clientsInSameNetwork = await _context.Query<ClientReference>().FromSql($"SELECT CS1.ClientId FROM ClientSession AS CS1 INNER JOIN ClientSession AS CS2 ON CS1.IpAddress = CS2.IpAddress WHERE CS1.ClientId = {targetId.ClientId} AND CS2.ClientId != {targetId.ClientId} GROUP BY CS2.ClientId ORDER BY CS2.CreatedOn DESC").ToListAsync();
                if(clientsInSameNetwork.Any())
                    context.ReportStatus("Send magic package over clients network...");

                this.LogInformation("{amount} clients with the same ip address were found.", clientsInSameNetwork.Count);

                foreach (var clientInSameNetwork in clientsInSameNetwork)
                {
                    if (_connectionManager.ClientConnections.TryGetValue(clientInSameNetwork.ClientId, out var connection))
                    {
                        this.LogDebug("Send magic package from client #{id}", clientInSameNetwork.ClientId);

                        try
                        {
                            await WakeOnLanResource.WakeOnLan(macAddress, connection);
                        }
                        catch (Exception e)
                        {
                            this.LogWarning(e, "Sending magic package from client #{id} failed.", clientInSameNetwork.ClientId);
                            continue;
                        }

                        this.LogInformation("Magic package sent from client #{id}.", clientInSameNetwork.ClientId);
                    }
                }
            }

            return Log(HttpStatusCode.OK);
        }

        private static void WakeOnLan(PhysicalAddress address)
        {
            // WOL packet is sent over UDP 255.255.255.0:40000.
            UdpClient client = new UdpClient();
            client.Connect(IPAddress.Broadcast, 40000);

            // WOL packet contains a 6-bytes trailer and 16 times a 6-bytes sequence containing the MAC address.
            byte[] packet = new byte[17 * 6];

            // Trailer of 6 times 0xFF.
            for (int i = 0; i < 6; i++)
                packet[i] = 0xFF;

            var addressBytes = address.GetAddressBytes();

            // Body of magic packet contains 16 times the MAC address.
            for (int i = 1; i <= 16; i++)
                for (int j = 0; j < 6; j++)
                    packet[i * 6 + j] = addressBytes[j];

            // Send WOL packet.
            client.Send(packet, packet.Length);
        }
    }

    public class ClientReference
    {
        public int ClientId { get; set; }
    }
}
