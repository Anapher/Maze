using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace ModulePacker
{
    public class IncludedReference
    {
        public List<PackageDependency> Dependencies { get; set; }
        public string ContentPath { get; set; }
        public string Id { get; set; }
    }
}