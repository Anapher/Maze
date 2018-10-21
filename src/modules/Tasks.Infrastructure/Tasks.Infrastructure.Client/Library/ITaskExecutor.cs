using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Infrastructure.Core.Commands;

namespace Tasks.Infrastructure.Client.Library
{
    public interface ITaskExecutor<in TCommandInfo> where TCommandInfo : CommandInfo
    {
        Task<HttpResponseMessage> InvokeAsync(TCommandInfo commandInfo, TaskExecutionContext context, CancellationToken cancellationToken);
    }

    public abstract class TaskExecutionContext
    {
        public abstract IServiceProvider Services { get; }
    }
}