using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Connection.Tasks.Triggers;

namespace Orcus.Client.Library.Tasks
{
    public interface ITriggerService<in TTransmissionInfo> where TTransmissionInfo : TriggerInfo
    {
        Task InvokeAsync(TTransmissionInfo transmissionInfo, TriggerContext context, CancellationToken cancellationToken);
    }
}