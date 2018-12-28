using System;

namespace Maze.Server.Connection
{
    public static class OfficalMazeRepository
    {
        public static Uri Uri { get; } = new Uri("https://api.nuget.org/v3/index.json");
    }
}