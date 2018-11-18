using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Common.Client
{
    public class ServerTestCommandExecutor : ITaskExecutor<ServerTestCommandInfo>
    {
        public async Task<HttpResponseMessage> InvokeAsync(ServerTestCommandInfo commandInfo, TaskExecutionContext context,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(400, cancellationToken);
                context.ReportProgress(i / 100d);
                context.ReportStatus($"Doing some stuff ({i}%)");
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}