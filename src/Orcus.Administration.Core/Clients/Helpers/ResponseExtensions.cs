using System.Net.Http;
using System.Threading.Tasks;

namespace Orcus.Administration.Core.Clients.Helpers
{
    internal static class ResponseExtensions
    {
        public static Task<T> Return<T>(this Task<HttpResponseMessage> response)
        {
            return new ResponseFactory<T>(response).ToResult();
        }

        public static ResponseFactory<T> Wrap<T>(this Task<HttpResponseMessage> response)
        {
            return new ResponseFactory<T>(response);
        }
    }
}