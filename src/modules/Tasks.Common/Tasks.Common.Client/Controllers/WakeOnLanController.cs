using Maze.Modules.Api;
using Maze.Modules.Api.Parameters;
using Maze.Modules.Api.Routing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tasks.Common.Client.Controllers
{
    [Route("wol")]
    public class WakeOnLanController : MazeController
    {
        [MazeGet]
        public async Task<IActionResult> WakeOnLan([FromQuery] string address)
        {
            await WakeOnLan(PhysicalAddress.Parse(address));
            return Ok();
        }

        private static async Task WakeOnLan(PhysicalAddress address)
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
            await client.SendAsync(packet, packet.Length);
        }

    }
}
