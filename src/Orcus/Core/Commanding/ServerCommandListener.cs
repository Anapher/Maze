using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Orcus.Service.Commander;
using Orcus.Sockets;
using Orcus.Sockets.Client;

namespace Orcus.Core.Commanding
{
    public class ServerCommandListener
    {
        private readonly OrcusSocketConnector _connector;
        private readonly WebSocketWrapper _orcusSocket;
        private readonly OrcusServer _orcusServer;
        private readonly ILifetimeScope _container;

        public ServerCommandListener(OrcusSocketConnector connector, WebSocketWrapper orcusSocket, OrcusServer orcusServer,
            ILifetimeScope container)
        {
            _connector = connector;
            _orcusSocket = orcusSocket;
            _orcusServer = orcusServer;
            _container = container;
        }

        public Task Listen()
        {
            _orcusServer.RequestReceived += OrcusServerOnRequestReceived;
            return _orcusSocket.ReceiveAsync();
        }

        private async void OrcusServerOnRequestReceived(object sender, OrcusRequestReceivedEventArgs e)
        {
            var context = new WebSocketOrcusContext(e)
            {
                RequestServices = new AutofacServiceProvider(_container),
                Connection = new WebSocketConnectionInfo(_connector)
            };

            await _container.Resolve<IOrcusRequestExecuter>().Execute(context);
            await _orcusServer.FinishResponse(e);
        }
    }
}