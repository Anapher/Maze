using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Frameworks;
using Orcus.Server.Connection.Modules;
using Orcus.Server.Service.Modules.Config.Base;

namespace Orcus.Server.Service.Modules.Config
{
    public class ModulesLock : JsonObjectFile<Dictionary<string, PackagesLock>>, IModulesLock
    {
        private readonly IImmutableDictionary<NuGetFramework, PackagesLock> _empty;

        public ModulesLock(string path) : base(path)
        {
            _empty = ImmutableDictionary<NuGetFramework, PackagesLock>.Empty;
            Modules = _empty;
        }

        public IImmutableDictionary<NuGetFramework, PackagesLock> Modules { get; private set; }

        public virtual async Task Reload()
        {
            var data = await Load();
            Modules = data == null
                ? _empty
                : data.ToImmutableDictionary(x => NuGetFramework.Parse(x.Key), x => x.Value, NuGetFramework.Comparer);
        }

        public virtual Task Add(NuGetFramework framework, PackagesLock packagesLock)
        {
            Modules = Modules.Add(framework, packagesLock);
            return Save(Modules);
        }

        public virtual Task Remove(NuGetFramework framework)
        {
            Modules = Modules.Remove(framework);
            return Save(Modules);
        }

        public Task Replace(IReadOnlyDictionary<NuGetFramework, PackagesLock> modules)
        {
            Modules = modules.ToImmutableDictionary(NuGetFramework.Comparer);
            return Save(Modules);
        }

        protected Task Save(IImmutableDictionary<NuGetFramework, PackagesLock> value)
        {
            return base.Save(value.ToDictionary(x => x.Key.ToString(), x => x.Value));
        }
    }
}