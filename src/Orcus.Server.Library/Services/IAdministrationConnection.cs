using Orcus.Sockets;

namespace Orcus.Server.Library.Services
{
    public interface IAdministrationConnection
    {
        int AccountId { get; }
        OrcusServer OrcusServer { get; }
    }
}