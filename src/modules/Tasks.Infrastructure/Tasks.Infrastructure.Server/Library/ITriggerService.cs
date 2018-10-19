using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Server.Library
{
    public interface ITriggerService<in TTransmissionInfo> where TTransmissionInfo : TriggerInfo
    {
        Task InvokeAsync(TTransmissionInfo transmissionInfo, TriggerContext context, CancellationToken cancellationToken);
    }
}