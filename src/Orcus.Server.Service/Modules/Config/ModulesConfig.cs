using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using Orcus.Server.Connection.JsonConverters;

namespace Orcus.Server.Service.Modules.Config
{
    public interface IModulesConfig
    {
        IImmutableList<PackageIdentity> Items { get; }
        string Path { get; }

        Task AddItem(PackageIdentity item);
        Task Reload();
        Task RemoveItem(PackageIdentity item);
    }

    public class ModulesConfig : ChangeableJsonFile<PackageIdentity>, IModulesConfig
    {
        public ModulesConfig(string path) : base(path)
        {
            JsonSettings.Converters.Add(new PackageIdentityConverter());
            JsonSettings.Converters.Add(new NuGetVersionConverter());
        }

        public override async Task AddItem(PackageIdentity item)
        {
            var modules = Items.ToList();
            var existingModule = modules.FirstOrDefault(x => x.Id == item.Id);
            if (existingModule != null)
                modules[modules.IndexOf(existingModule)] = item;
            else
                modules.Add(item);

            using (await ReaderWriterLock.WriterLockAsync())
            {
                File.WriteAllText(Path, Serialize(modules));
            }

            Items = modules.ToImmutableList();
        }
    }
}