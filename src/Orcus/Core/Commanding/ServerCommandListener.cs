using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Orcus.Service.Commander;
using Orcus.Sockets;

namespace Orcus.Core.Commanding
{
    public class ServerCommandListener
    {
        private readonly OrcusSocket _orcusSocket;
        private readonly OrcusServer _orcusServer;
        private readonly ILifetimeScope _container;

        public ServerCommandListener(OrcusSocket orcusSocket, OrcusServer orcusServer, ILifetimeScope container)
        {
            _orcusSocket = orcusSocket;
            _orcusServer = orcusServer;
            _container = container;
        }

        public Task Listen()
        {
            _orcusServer.RequestReceived += OrcusServerOnRequestReceived;
            return _orcusSocket.ReceiveAsync();
        }

        private void OrcusServerOnRequestReceived(object sender, OrcusRequestReceivedEventArgs e)
        {
            var context = new WebSocketOrcusContext(e) {RequestServices = new AutofacServiceProvider(_container)};
            _container.Resolve<IOrcusRequestExecuter>().Execute(context);
        }
    }
}