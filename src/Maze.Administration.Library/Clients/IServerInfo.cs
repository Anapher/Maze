using System;

namespace Maze.Administration.Library.Clients
{
    public interface IServerInfo
    {
        Uri ServerUri { get; }
    }
}