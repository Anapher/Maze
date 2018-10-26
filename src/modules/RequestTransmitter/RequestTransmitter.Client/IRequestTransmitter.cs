using System.Net.Http;
using System.Threading.Tasks;

namespace RequestTransmitter.Client
{
    public interface IRequestTransmitter
    {
        Task<bool> Transmit(HttpRequestMessage requestMessage);
        Task<bool> Transmit<TResponseCallback>(HttpRequestMessage requestMessage) where TResponseCallback : IResponseCallback;
    }
}