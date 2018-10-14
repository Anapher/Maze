using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Server.Connection.Tasks.Commands;

namespace Orcus.Client.Library.Tasks
{
    public interface ITaskExecutor<in TCommandInfo> where TCommandInfo : CommandInfo
    {
        Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TaskExecutionContext context, CancellationToken cancellationToken);
    }

    public abstract class TaskExecutionContext
    {
    }
}