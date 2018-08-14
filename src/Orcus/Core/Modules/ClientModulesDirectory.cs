using System;
using Microsoft.Extensions.Options;
using Orcus.ModuleManagement;
using Orcus.Options;

namespace Orcus.Core.Modules
{
    public class ClientModulesDirectory : ModulesDirectory
    {
        public ClientModulesDirectory(IOptions<ModulesOptions> options) : base(
            new VersionFolderPathResolverFlat(Environment.ExpandEnvironmentVariables(options.Value.LocalPath)))
        {
        }
    }
}