using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NuGet.Packaging.Core;
using Orcus.Server.Connection.Modules;

namespace Orcus.Options
{
    public class ModulesOptions
    {
        public string LocalPath { get; set; }
        public string TempPath { get; set; }
        public PackagesLock PackagesLock { get; set; }
    }
}