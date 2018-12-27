using Orcus.Server.Connection.Modules;

namespace Orcus.Client.Library.Services
{
    public interface IServerConnection
    {
        IOrcusRestClient RestClient { get; }
        PackagesLock PackagesLock { get; }
    }
}
