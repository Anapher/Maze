using System;
using Maze.Administration.Library.Clients;

namespace Maze.Administration.Core
{
    public class ServerInfo : IServerInfo
    {
        public Uri ServerUri { get; set; }
    }
}