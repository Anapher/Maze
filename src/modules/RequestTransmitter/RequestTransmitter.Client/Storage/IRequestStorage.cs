using System.Net.Http;
using System.Threading.Tasks;

namespace RequestTransmitter.Client.Storage
{
    public interface IRequestStorage
    {
        bool HasEntries { get; }
        Task<HttpRequestMessage> Peek();
        Task Pop();
        Task Push(HttpRequestMessage requestMessage);
    }
}