using System;
using System.Collections.Immutable;
using NuGet.Packaging.Core;
using Maze.Server.Service.Modules.Loader;

namespace Maze.Server.ControllersBase
{
    public interface IModuleControllerProvider
    {
        IImmutableDictionary<PackageIdentity, IImmutableList<Type>> Controllers { get; }
    }

    public class ModuleControllerProvider : IModuleControllerProvider
    {
        public ModuleControllerProvider(ModuleTypeMap moduleTypeMap)
        {
            Controllers = moduleTypeMap.Controllers.ToImmutableDictionary(x => x.Key,
                x => (IImmutableList<Type>) x.Value.ToImmutableList());
        }

        public ModuleControllerProvider()
        {
            Controllers = ImmutableDictionary<PackageIdentity, IImmutableList<Type>>.Empty;
        }

        public IImmutableDictionary<PackageIdentity, IImmutableList<Type>> Controllers { get; }
    }
}