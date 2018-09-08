using System.Threading.Tasks;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Request;
using Orcus.Modules.Api.Response;

namespace Orcus.Service.Commander
{
    /// <summary>
    ///     Take an <see cref="OrcusRequest" /> and execute it, responding with an <see cref="OrcusResponse" />
    /// </summary>
    public interface IOrcusRequestExecuter
    {
        /// <summary>
        ///     Execute the given <see cref="OrcusRequest" />
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="channelServer">The server that manages the active channel</param>
        /// <returns>Return the result of the request</returns>
        Task Execute(OrcusContext context, IChannelServer channelServer);
    }
}