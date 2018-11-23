using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Client.Commands.Base;
using Tasks.Common.Client.Utilities;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Management.Library;

namespace Tasks.Common.Client.Commands
{
    public class DownloadAndExecuteCommandExecutor : ProcessCommandExecutorBase, ITaskExecutor<DownloadAndExecuteCommandInfo>
    {
        public async Task<HttpResponseMessage> InvokeAsync(DownloadAndExecuteCommandInfo commandInfo, TaskExecutionContext context, CancellationToken cancellationToken)
        {
            DirectoryInfo tempDirectory = null;
            do
            {
                tempDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            } while (tempDirectory.Exists);

            var filename = Path.Combine(tempDirectory.FullName, commandInfo.FileName);
            var processStartInfo = commandInfo.ToStartInfo(filename);

            if (commandInfo.WaitForExit)
            {
                tempDirectory.Create();
                try
                {
                    using (var fileStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite, 8192, FileOptions.DeleteOnClose))
                    {
                        if (!await commandInfo.FileSource.WriteTo(fileStream, context, this))
                            return Log(HttpStatusCode.InternalServerError);

                        return await StartProcess(processStartInfo, true, cancellationToken);
                    }
                }
                finally
                {
                    tempDirectory.Delete(true);
                }
            }
            else
            {
                FileHelper.RemoveOnReboot(filename);
                FileHelper.RemoveOnReboot(tempDirectory.FullName);

                tempDirectory.Create();
                using (var fileStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write))
                {
                    if (!await commandInfo.FileSource.WriteTo(fileStream, context, this))
                        return Log(HttpStatusCode.InternalServerError);
                }

                return await StartProcess(processStartInfo, false, cancellationToken);
            }
        }
    }
}
