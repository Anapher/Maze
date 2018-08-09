using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orcus.Client.Library.Services
{
    public interface IOrcusRestClient
    {
        Uri BaseUri { get; }
        string Jwt { get; }

        Task<HttpResponseMessage> SendMessage(HttpRequestMessage request);
    }
}