using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Common.Client.Commands
{
    public class ShutdownCommandExecutor : TaskExecutor, ITaskExecutor<ShutdownCommandInfo>
    {
        public Task<HttpResponseMessage> InvokeAsync(ShutdownCommandInfo commandInfo, TaskExecutionContext context, CancellationToken cancellationToken)
        {
            context.ReportStatus("Shutting down...");
            context.AfterExecutionCallback = ExecuteShutdown;

            return Task.FromResult(Ok());
        }

        private Task ExecuteShutdown()
        {
            ExecuteShutdown("/s /t 0");
            return Task.CompletedTask;
        }

        private void ExecuteShutdown(string arguments)
        {
            var psi = new ProcessStartInfo("shutdown.exe", arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            var process = Process.Start(psi);
            if (process == null)
                throw new InvalidOperationException("Unable to start shutdown.exe");
        }
    }
}
