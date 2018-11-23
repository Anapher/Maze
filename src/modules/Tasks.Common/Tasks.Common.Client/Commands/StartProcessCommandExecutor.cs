using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Client.Commands.Base;
using Tasks.Common.Client.Utilities;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Common.Client.Commands
{
    public class StartProcessCommandExecutor : ProcessCommandExecutorBase, ITaskExecutor<StartProcessCommandInfo>
    {
        public Task<HttpResponseMessage> InvokeAsync(StartProcessCommandInfo commandInfo, TaskExecutionContext context, CancellationToken cancellationToken)
        {
            return StartProcess(commandInfo.ToStartInfo(commandInfo.FileName), commandInfo.WaitForExit, cancellationToken);
        }
    }
}
