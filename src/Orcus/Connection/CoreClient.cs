using System;
using System.Threading.Tasks;

namespace Orcus.Connection
{
    public class CoreClient
    {
        private readonly ServerConnector _serverConnector;

        public CoreClient()
        {
            _serverConnector = new ServerConnector(null, null);
        }

        public async Task Run()
        {
            var connection = await _serverConnector.TryConnect(new Uri(""));
            if (connection != null)
            {

            }
        }
    }
}