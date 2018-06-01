using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace Orcus.Server.Service.Modules.Loader
{
    public class ModuleTypeMap
    {
        public ModuleTypeMap()
        {
            Controllers = new ConcurrentDictionary<PackageIdentity, List<Type>>();
            Startup = new ConcurrentBag<Type>();
        }

        public ConcurrentDictionary<PackageIdentity, List<Type>> Controllers { get; }
        public ConcurrentBag<Type> Startup { get; }
    }
}