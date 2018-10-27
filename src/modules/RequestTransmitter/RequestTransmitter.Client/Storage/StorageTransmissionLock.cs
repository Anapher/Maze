using Nito.AsyncEx;

namespace RequestTransmitter.Client.Storage
{
    public class StorageTransmissionLock
    {
        public StorageTransmissionLock()
        {
            AsyncLock = new AsyncLock();
        }

        public AsyncLock AsyncLock { get; }
    }
}