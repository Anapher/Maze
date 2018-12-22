using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Server.Library;

namespace Tasks.Common.Server.Commands
{
    public class ServerTestCommandExecutor : ITaskExecutor<ServerTestCommandInfo>
    {
        public async Task<HttpResponseMessage> InvokeAsync(ServerTestCommandInfo commandInfo, TargetId targetId, TaskExecutionContext context,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(100, cancellationToken);
                context.ReportProgress(i / 100d);
                context.ReportStatus($"Doing some stuff ({i}%)");
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}