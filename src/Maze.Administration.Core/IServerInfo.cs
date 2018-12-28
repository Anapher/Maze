using System;

namespace Maze.Administration.Core
{
    public interface IServerInfo
    {
        Uri ServerUri { get; }
    }

    public class ServerInfo : IServerInfo
    {
        public Uri ServerUri { get; set; }
    }
}