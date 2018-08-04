using System.Net.Http;

namespace Orcus.Client.Library.Interfaces
{
    public interface IConfigureHttpMessageHandler
    {
        HttpMessageHandler Configure(HttpMessageHandler handler);
    }
}