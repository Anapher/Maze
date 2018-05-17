using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Orcus.Server.Service.Modules.Config
{
    public abstract class ChangeableFile<TItem>
    {
        protected readonly AsyncReaderWriterLock ReaderWriterLock = new AsyncReaderWriterLock();

        protected ChangeableFile(string path)
        {
            Path = path;
            Items = new ImmutableArray<TItem>();
        }

        public string Path { get; }
        public IImmutableList<TItem> Items { get; protected set; }

        public virtual async Task Reload()
        {
            if (File.Exists(Path))
                using (await ReaderWriterLock.ReaderLockAsync())
                {
                    var list = Deserialize(File.ReadAllText(Path));
                    Items = list.ToImmutableList();
                }
            else Items = new ImmutableArray<TItem>();
        }

        public virtual async Task AddItem(TItem item)
        {
            var items = Items.ToList();
            items.Add(item);

            using (await ReaderWriterLock.WriterLockAsync())
            {
                File.WriteAllText(Path, Serialize(items));
            }

            Items = items.ToImmutableList();
        }

        public virtual async Task RemoveItem(TItem item)
        {
            var items = Items.ToList();
            items.Remove(item);

            using (await ReaderWriterLock.WriterLockAsync())
            {
                File.WriteAllText(Path, Serialize(items));
            }

            Items = items.ToImmutableList();
        }

        protected abstract List<TItem> Deserialize(string content);
        protected abstract string Serialize(List<TItem> items);
    }
}