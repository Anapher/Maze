// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Maze.Modules.Api.Request;

namespace Maze.Service.Commander.Infrastructure
{
    /// <summary>
    ///     Extension methods for enabling buffering in an <see cref="MazeRequest" />.
    /// </summary>
    public static class HttpRequestRewindExtensions
    {
        /// <summary>
        ///     Ensure the <paramref name="request" /> <see cref="MazeRequest.Body" /> can be read multiple times. Normally
        ///     buffers request bodies in memory; writes requests larger than 30K bytes to disk.
        /// </summary>
        /// <param name="request">The <see cref="MazeRequest" /> to prepare.</param>
        /// <remarks>
        ///     Temporary files for larger requests are written to the location named in the <c>ASPNETCORE_TEMP</c>
        ///     environment variable, if any. If that environment variable is not defined, these files are written to the
        ///     current user's temporary folder. Files are automatically deleted at the end of their associated requests.
        /// </remarks>
        public static void EnableBuffering(this MazeRequest request)
        {
            request.EnableRewind();
        }
    }
}