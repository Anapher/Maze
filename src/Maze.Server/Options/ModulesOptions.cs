using System;
using System.Collections.Generic;
using Maze.Server.Service.Modules;

namespace Maze.Server.Options
{
    public class ModulesOptions : IModuleProjectConfig
    {
        public string Directory { get; set; }
        public string ConfigDirectory { get; set; }
        public string ModulesFile { get; set; }
        public string ModulesLock { get; set; }
        public Uri[] PrimarySources { get; set; }
        public Uri[] DependencySources { get; set; }
        public Dictionary<string, string> Frameworks { get; set; }
    }
}