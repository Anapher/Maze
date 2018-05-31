using System;

namespace Orcus.Server.Options
{
    public class ModulesOptions
    {
        public string Directory { get; set; }
        public string ConfigDirectory { get; set; }
        public string ModulesFile { get; set; }
        public string ModulesLock { get; set; }
        public Uri[] PrimarySources { get; set; }
        public Uri[] DependencySources { get; set; }
    }
}