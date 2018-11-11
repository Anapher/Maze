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
            await Task.Delay(10000, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}