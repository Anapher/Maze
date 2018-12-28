using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using Maze.Server.Connection.JsonConverters;
using Maze.Server.Service.Modules.Config.Base;
using Maze.Server.Service.Modules.Extensions;

namespace Maze.Server.Service.Modules.Config
{
    public class ModulesConfig : JsonObjectFile<IReadOnlyList<PackageIdentity>>, IModulesConfig
    {
        public ModulesConfig(string path) : base(path)
        {
            Modules = ImmutableList<PackageIdentity>.Empty;

            JsonSettings.Converters.Add(new PackageIdentityConverter());
        }

        public IImmutableList<PackageIdentity> Modules { get; private set; }

        public virtual async Task Reload()
        {
            var data = await Load();
            Modules = data?.ToImmutableList() ?? ImmutableList<PackageIdentity>.Empty;
        }

        public virtual Task Add(PackageIdentity id)
        {
            if (Modules.Any(x => x.IsSameId(id)))
                throw new ArgumentException($"Module '{id}' already exists.", nameof(id));

            Modules = Modules.Add(id);
            return Save(Modules);
        }

        public virtual Task Remove(PackageIdentity id)
        {
            var module = Modules.FirstOrDefault(id.Equals);
            if (module == null)
                throw new ArgumentException($"Module '{id}' does not exist.", nameof(id));

            Modules = Modules.Remove(module);
            return Save(Modules);
        }

        public Task Replace(IEnumerable<PackageIdentity> modules)
        {
            Modules = modules.ToImmutableList();
            return Save(Modules);
        }
    }
}