using Maze.Client.Library.Clients;
using System;

namespace Maze.Client.Library.Services
{
    public interface IMazeRestClient : IRestClient
    {
        Uri BaseUri { get; }
        string Jwt { get; }
    }
}