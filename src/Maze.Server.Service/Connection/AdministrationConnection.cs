using System;
using System.Threading.Tasks;
using Maze.Server.Library.Services;
using Maze.Sockets;

namespace Maze.Server.Service.Connection
{
    public class AdministrationConnection : IAdministrationConnection, IDisposable
    {
        public AdministrationConnection(int accountId, WebSocketWrapper webSocket, MazeServer mazeServer)
        {
            AccountId = accountId;
            WebSocket = webSocket;
            MazeServer = mazeServer;
        }

        public void Dispose()
        {
            WebSocket?.Dispose();
            MazeServer?.Dispose();
        }

        public int AccountId { get; }
        public WebSocketWrapper WebSocket { get; }
        public MazeServer MazeServer { get; }

        public Task BeginListen()
        {
            return WebSocket.ReceiveAsync();
        }
    }
}