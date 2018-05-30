using System.Collections.Immutable;
using System.Net.Http;

namespace Orcus.Server.Library
{
    public interface IHttpClientService
    {
        HttpClient Client { get; }
    }
}