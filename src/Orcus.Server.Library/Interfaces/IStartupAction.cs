using Orcus.Modules.Api;

namespace Orcus.Server.Library.Interfaces
{
    /// <summary>
    ///     An action that will be invoked when the server started and configured the IoC container.
    /// </summary>
    public interface IStartupAction : IActionInterface
    {
    }
}