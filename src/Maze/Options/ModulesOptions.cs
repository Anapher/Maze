using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NuGet.Packaging.Core;
using Maze.Server.Connection.Modules;

namespace Maze.Options
{
    public class ModulesOptions
    {
        public string LocalPath { get; set; }
        public string TempPath { get; set; }
        public PackagesLock PackagesLock { get; set; }
    }
}