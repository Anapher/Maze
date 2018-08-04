using System.Net.Http;

namespace Orcus.Client.Library.Services
{
    public interface IHttpClientService
    {
        HttpClient Client { get; }
    }
}