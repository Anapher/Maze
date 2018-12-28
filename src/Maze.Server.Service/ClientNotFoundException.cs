using System;

namespace Maze.Server.Service
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException(int clientId) : base($"The client with id '{clientId}' was not found.")
        {
        }
    }
}