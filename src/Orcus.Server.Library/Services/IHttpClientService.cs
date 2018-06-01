using System.Net.Http;

namespace Orcus.Server.Library.Services
{
    public interface IHttpClientService
    {
        HttpClient Client { get; }
    }
}