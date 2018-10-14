using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Connection.Tasks.Commands;

namespace Orcus.Server.Library.Tasks
{
    public interface ITaskExecutor<in TCommandInfo> where TCommandInfo : CommandInfo
    {
        Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TargetId targetId, TaskExecutionContext context,
            CancellationToken cancellationToken);
    }
}