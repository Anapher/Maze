using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tasks.Common.Client.Commands.Base;
using Tasks.Common.Client.Utilities;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Client.Library;

namespace Tasks.Common.Client.Commands
{
    public class DownloadAndExecuteCommandExecutor : ProcessCommandExecutorBase, ITaskExecutor<DownloadAndExecuteCommandInfo>
    {
        public async Task<HttpResponseMessage> InvokeAsync(DownloadAndExecuteCommandInfo commandInfo, TaskExecutionContext context,
            CancellationToken cancellationToken)
        {
            DirectoryInfo tempDirectory;
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
                    using (var fileStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite, 8192,
                        FileOptions.DeleteOnClose))
                    {
                        if (!await commandInfo.FileSource.WriteTo(fileStream, context, this))
                            return Log(HttpStatusCode.InternalServerError);

                        return await StartProcess(processStartInfo, waitForExit: true, cancellationToken);
                    }
                }
                finally
                {
                    tempDirectory.Delete(true);
                }
            }

            FileHelper.RemoveOnReboot(filename);
            FileHelper.RemoveOnReboot(tempDirectory.FullName);

            tempDirectory.Create();
            using (var fileStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write))
            {
                if (!await commandInfo.FileSource.WriteTo(fileStream, context, this))
                    return Log(HttpStatusCode.InternalServerError);
            }

            return await StartProcess(processStartInfo, waitForExit: false, cancellationToken);
        }
    }
}