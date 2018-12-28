using System;
using System.Collections.Generic;

namespace Maze.Server.Service.Modules
{
    public interface IModuleProjectConfig
    {
        string Directory { get; set; }
        Uri[] PrimarySources { get; set; }
        Uri[] DependencySources { get; set; }
        Dictionary<string, string> Frameworks { get; set; }
    }
}
