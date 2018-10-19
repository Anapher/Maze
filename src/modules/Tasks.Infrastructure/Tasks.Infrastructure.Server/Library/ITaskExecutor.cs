using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Server.Library
{
    public interface ITaskExecutor<in TCommandInfo> where TCommandInfo : CommandInfo
    {
        Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TargetId targetId, TaskExecutionContext context,
            CancellationToken cancellationToken);
    }
}