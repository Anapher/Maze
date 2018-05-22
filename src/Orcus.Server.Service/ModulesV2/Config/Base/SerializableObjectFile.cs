using System.IO;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Orcus.Server.Service.ModulesV2.Config.Base
{
    public abstract class SerializableObjectFile<TObject>
    {
        protected readonly AsyncReaderWriterLock ReaderWriterLock = new AsyncReaderWriterLock();

        protected SerializableObjectFile(string path)
        {
            Path = path;
        }

        public string Path { get; }

        protected virtual async Task Save(TObject value)
        {
            using (await ReaderWriterLock.WriterLockAsync())
            {
                File.WriteAllText(Path, Serialize(value));
            }
        }

        protected virtual async Task<TObject> Load()
        {
            if (File.Exists(Path))
                using (await ReaderWriterLock.ReaderLockAsync())
                {
                    return Deserialize(File.ReadAllText(Path));
                }

            return default(TObject);
        }
        
        protected abstract TObject Deserialize(string content);
        protected abstract string Serialize(TObject value);
    }
}