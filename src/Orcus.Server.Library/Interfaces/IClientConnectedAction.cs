using Orcus.Modules.Api;
using Orcus.Server.Library.Services;

namespace Orcus.Server.Library.Interfaces
{
    /// <summary>
    ///     An action that will be invoked once a client connects to the server.
    /// </summary>
    public interface IClientConnectedAction : IActionInterface<IClientConnection>
    {
    }
}