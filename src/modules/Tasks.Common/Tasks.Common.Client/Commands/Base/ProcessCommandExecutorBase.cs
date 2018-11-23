using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Client.Utilities;
using Tasks.Infrastructure.Management.Library;

namespace Tasks.Common.Client.Commands.Base
{
    public abstract class ProcessCommandExecutorBase : LoggingTaskExecutor
    {
        protected async Task<HttpResponseMessage> StartProcess(ProcessStartInfo startInfo, bool waitForExit, CancellationToken cancellationToken)
        {
            this.LogDebug("Starting process \"{filename}\"...", startInfo.FileName);

            var process = Process.Start(startInfo);
            if (process == null)
                return Log(HttpStatusCode.InternalServerError);

            this.LogInformation("Process started successfully");

            using (process)
            {
                if (!waitForExit)
                    return Log(HttpStatusCode.OK);

                process.OutputDataReceived += ProcessOnOutputDataReceived;
                process.ErrorDataReceived += ProcessOnErrorDataReceived;

                await process.WaitForExitAsync(cancellationToken);
            }

            return Log(HttpStatusCode.OK);
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.LogError(e.Data);
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.LogInformation(e.Data);
        }
    }
}
