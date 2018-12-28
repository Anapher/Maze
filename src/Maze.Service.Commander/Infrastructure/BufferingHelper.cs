// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.AspNetCore.WebUtilities;
using Maze.Modules.Api.Extensions;
using Maze.Modules.Api.Request;

namespace Maze.Service.Commander.Infrastructure
{
    public static class BufferingHelper
    {
        internal const int DefaultBufferThreshold = 1024 * 30;

        private readonly static Func<string> _getTempDirectory = () => TempDirectory;

        private static string _tempDirectory;

        public static string TempDirectory
        {
            get
            {
                if (_tempDirectory == null)
                {
                    // Look for folders in the following order.
                    var temp = Environment.GetEnvironmentVariable("ASPNETCORE_TEMP") ??     // ASPNETCORE_TEMP - User set temporary location.
                               Path.GetTempPath();                                      // Fall back.

                    if (!Directory.Exists(temp))
                    {
                        // TODO: ???
                        throw new DirectoryNotFoundException(temp);
                    }

                    _tempDirectory = temp;
                }

                return _tempDirectory;
            }
        }

        public static MazeRequest EnableRewind(this MazeRequest request, int bufferThreshold = DefaultBufferThreshold, long? bufferLimit = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var body = request.Body;
            if (!body.CanSeek)
            {
                var fileStream = new Microsoft.AspNetCore.WebUtilities.FileBufferingReadStream(body, bufferThreshold, bufferLimit, _getTempDirectory);
                request.Body = fileStream;
                request.Context.Response.RegisterForDispose(fileStream);
            }
            return request;
        }

        public static MultipartSection EnableRewind(this MultipartSection section, Action<IDisposable> registerForDispose,
            int bufferThreshold = DefaultBufferThreshold, long? bufferLimit = null)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }
            if (registerForDispose == null)
            {
                throw new ArgumentNullException(nameof(registerForDispose));
            }

            var body = section.Body;
            if (!body.CanSeek)
            {
                var fileStream = new Microsoft.AspNetCore.WebUtilities.FileBufferingReadStream(body, bufferThreshold, bufferLimit, _getTempDirectory);
                section.Body = fileStream;
                registerForDispose(fileStream);
            }
            return section;
        }
    }
}