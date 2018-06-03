// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Response;
using Orcus.Modules.Api.Services;
using Orcus.Server.Service.Logging;

namespace Orcus.Server.Service.Infrastructure
{
    public class FileStreamResultExecutor : FileResultExecutorBase, IActionResultExecutor<FileStreamResult>
    {
        public FileStreamResultExecutor(ILogger<FileStreamResultExecutor> logger) : base(logger)
        {
        }

        public virtual async Task ExecuteAsync(ActionContext context, FileStreamResult result)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (result == null)
                throw new ArgumentNullException(nameof(result));

            using (result.FileStream)
            {
                Logger.ExecutingFileResult(result);

                long? fileLength = null;
                if (result.FileStream.CanSeek)
                {
                    fileLength = result.FileStream.Length;
                }

                var (range, rangeLength, serveBody) = SetHeadersAndLog(
                    context,
                    result,
                    fileLength,
                    result.EnableRangeProcessing,
                    result.LastModified,
                    result.EntityTag);

                if (!serveBody)
                {
                    return;
                }

                await WriteFileAsync(context, result, range, rangeLength);
            }
        }

        protected virtual Task WriteFileAsync(ActionContext context, FileStreamResult result,
            RangeItemHeaderValue range, long rangeLength)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (range != null && rangeLength == 0)
                return Task.CompletedTask;

            if (range != null)
                Logger.LogDebug("Writing range to body");

            return WriteFileAsync(context.Context, result.FileStream, range, rangeLength);
        }
    }
}
