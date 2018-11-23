using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orcus.Server.Connection.Utilities;
using Tasks.Common.Client.Utilities;
using Tasks.Common.Shared.Commands;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Management.Library;

namespace Tasks.Common.Client.Commands
{
    public class DownloadCommandExecutor : LoggingTaskExecutor, ITaskExecutor<DownloadCommandInfo>
    {
        public async Task<HttpResponseMessage> InvokeAsync(DownloadCommandInfo commandInfo, TaskExecutionContext context,
            CancellationToken cancellationToken)
        {
            var targetFile = new FileInfo(commandInfo.TargetPath);

            FileStream targetStream;
            if (targetFile.Exists)
            {
                switch (commandInfo.FileExistsBehavior)
                {
                    case FileExistsBehavior.NoAction:
                        this.LogInformation("File {path} already exists. Return.", targetFile.FullName);
                        return Log(HttpStatusCode.OK);
                    case FileExistsBehavior.ReplaceFile:
                        this.LogInformation("File {path} already exists. Overwrite file...", targetFile.FullName);

                        try
                        {
                            targetStream = new FileStream(targetFile.FullName, FileMode.Create, FileAccess.ReadWrite);
                        }
                        catch (Exception e)
                        {
                            this.LogError(e, "Overwriting the file failed.");
                            return Log(HttpStatusCode.InternalServerError);
                        }

                        break;
                    case FileExistsBehavior.SaveWithDifferentName:
                        this.LogInformation("File {path} already exists. Searching for a new name...", targetFile.FullName);

                        targetStream = null;
                        NameGeneratorUtilities.MakeUnique(targetFile.Name, " ([N])", name =>
                        {
                            try
                            {
                                var filename = Path.Combine(targetFile.DirectoryName, name);
                                targetStream = new FileStream(filename, FileMode.CreateNew, FileAccess.ReadWrite);
                                targetFile = new FileInfo(filename);
                                return true;
                            }
                            catch (Exception)
                            {
                                return false;
                            }
                        });
                        break;
                    case FileExistsBehavior.AttemptToReplaceElseSaveWithDifferentName:
                        this.LogInformation("File {path} already exists. Overwrite file...", targetFile.FullName);

                        try
                        {
                            targetStream = new FileStream(targetFile.FullName, FileMode.Create, FileAccess.ReadWrite);
                        }
                        catch (Exception e)
                        {
                            this.LogWarning(e, "Overwriting the file failed. Searching for a new name...");

                            targetStream = null;
                            NameGeneratorUtilities.MakeUnique(targetFile.Name, " ([N])", name =>
                            {
                                try
                                {
                                    var filename = Path.Combine(targetFile.DirectoryName, name);
                                    targetStream = new FileStream(filename, FileMode.CreateNew, FileAccess.ReadWrite);
                                    targetFile = new FileInfo(filename);
                                    return true;
                                }
                                catch (Exception)
                                {
                                    return false;
                                }
                            });
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                this.LogInformation("File {file} does not exist yet. Creating file...", commandInfo.TargetPath);

                if (targetFile.Directory?.Exists == false)
                {
                    this.LogInformation("Creating directory {directory}", targetFile.DirectoryName);
                    targetFile.Directory.Create();
                }

                targetStream = new FileStream(targetFile.FullName, FileMode.CreateNew, FileAccess.ReadWrite);
            }

            if (!await commandInfo.FileSource.WriteTo(targetStream, context, this))
                return Log(HttpStatusCode.InternalServerError);

            return Log(HttpStatusCode.OK);
        }
    }
}