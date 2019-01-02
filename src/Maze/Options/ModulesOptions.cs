using System.IO;
using Maze.Server.Connection.Modules;

namespace Maze.Options
{
    public class ModulesOptions
    {
        public string LocalPath { get; set; } = Path.Combine(Program.ConfigDirectory, "modules");
        public string TempPath { get; set; } = Path.Combine(Program.ConfigDirectory, "temp");
        public string ModulesLockPath { get; set; } = Path.Combine(Program.ConfigDirectory, "modules.lock");
    }
}