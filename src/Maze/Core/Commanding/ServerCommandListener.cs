using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Maze.Service.Commander;
using Maze.Sockets;
using Maze.Sockets.Client;

namespace Maze.Core.Commanding
{
    public class ServerCommandListener
    {
        private readonly MazeSocketConnector _connector;
        private readonly WebSocketWrapper _mazeSocket;
        private readonly MazeServer _mazeServer;
        private readonly ILifetimeScope _container;

        public ServerCommandListener(MazeSocketConnector connector, WebSocketWrapper mazeSocket, MazeServer mazeServer,
            ILifetimeScope container)
        {
            _connector = connector;
            _mazeSocket = mazeSocket;
            _mazeServer = mazeServer;
            _container = container;
        }

        public Task Listen()
        {
            _mazeServer.RequestReceived += MazeServerOnRequestReceived;
            return _mazeSocket.ReceiveAsync();
        }

        private async void MazeServerOnRequestReceived(object sender, MazeRequestReceivedEventArgs e)
        {
            var context = new WebSocketMazeContext(e)
            {
                RequestServices = new AutofacServiceProvider(_container),
                Connection = new WebSocketConnectionInfo(_connector)
            };

            await _container.Resolve<IMazeRequestExecuter>().Execute(context, _mazeServer);
            await _mazeServer.FinishResponse(e);
        }
    }
}