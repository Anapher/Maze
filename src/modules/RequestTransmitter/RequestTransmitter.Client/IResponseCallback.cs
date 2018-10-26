using System.Net.Http;
using System.Threading.Tasks;

namespace RequestTransmitter.Client
{
    public interface IResponseCallback
    {
        Task ResponseReceived(HttpResponseMessage responseMessage);
    }
}
