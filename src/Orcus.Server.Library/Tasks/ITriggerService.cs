using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Connection.Tasks.Transmission;

namespace Orcus.Server.Library.Tasks
{
    public interface ITriggerService<in TTransmissionInfo> where TTransmissionInfo : TransmissionInfo
    {
        Task InvokeAsync(TTransmissionInfo transmissionInfo, TriggerContext context, CancellationToken cancellationToken);
    }
}