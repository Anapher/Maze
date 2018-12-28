using System;
using Microsoft.Extensions.Options;
using Maze.ModuleManagement;
using Maze.Options;

namespace Maze.Core.Modules
{
    public class ClientModulesDirectory : ModulesDirectory
    {
        public ClientModulesDirectory(IOptions<ModulesOptions> options) : base(
            new VersionFolderPathResolverFlat(Environment.ExpandEnvironmentVariables(options.Value.LocalPath)))
        {
        }
    }
}