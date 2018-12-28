using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Maze.Modules.Api.Request;
using Maze.Modules.Api.Response;

namespace Maze.Modules.Api
{
    /// <summary>
    ///     Encapsulates all information about an individual Maze request.
    /// </summary>
    public abstract class MazeContext
    {
        public abstract object Caller { get; set; }

        /// <summary>
        ///     Gets the <see cref="MazeRequest" /> object for this request.
        /// </summary>
        public abstract MazeRequest Request { get; set; }

        /// <summary>
        ///     Gets the <see cref="MazeResponse" /> object for this request.
        /// </summary>
        public abstract MazeResponse Response { get; set; }

        /// <summary>
        ///     Gets information about the underlying connection for this request.
        /// </summary>
        public abstract ConnectionInfo Connection { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="T:System.IServiceProvider" /> that provides access to the request's service container.
        /// </summary>
        public abstract IServiceProvider RequestServices { get; set; }

        /// <summary>
        ///     Notifies when the connection underlying this request is aborted and thus request operations should be
        ///     cancelled.
        /// </summary>
        public abstract CancellationToken RequestAborted { get; set; }

        /// <summary>Aborts the connection underlying this request.</summary>
        public abstract void Abort();
    }
}