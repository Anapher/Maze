using System.Threading.Tasks;
using Orcus.Sockets;

namespace Orcus.Server.OrcusSockets
{
    /// <summary>
    ///     Activator of an <see cref="OrcusSocket" />
    /// </summary>
    public interface IOrcusSocketFeature
    {
        /// <summary>
        ///     Upgrade the connection and create the <see cref="OrcusSocket" />
        /// </summary>
        /// <returns>Return the created <see cref="OrcusSocket" /></returns>
        Task<OrcusSocket> AcceptAsync();
    }
}