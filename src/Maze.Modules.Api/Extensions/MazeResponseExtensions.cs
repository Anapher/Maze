using System;
using System.Threading.Tasks;
using Maze.Modules.Api.Response;

namespace Maze.Modules.Api.Extensions
{
    public static class MazeResponseExtensions
    {
        private static readonly Func<object, Task> DisposeDelegate = disposable =>
        {
            ((IDisposable) disposable).Dispose();
            return Task.CompletedTask;
        };

        private static readonly Func<object, Task> CallbackDelegate = callback => ((Func<Task>) callback)();

        /// <summary>
        ///     Registers an object for disposal by the host once the request has finished processing.
        /// </summary>
        /// <param name="response">The <see cref="MazeResponse" /></param>
        /// <param name="disposable">The object to be disposed.</param>
        public static void RegisterForDispose(this MazeResponse response, IDisposable disposable)
        {
            response.OnCompleted(DisposeDelegate, disposable);
        }

        /// <summary>
        ///     Adds a delegate to be invoked just before response headers will be sent to the client.
        /// </summary>
        /// <param name="response">The <see cref="MazeResponse" /></param>
        /// <param name="callback">The delegate to execute.</param>
        public static void OnStarting(this MazeResponse response, Func<Task> callback)
        {
            response.OnStarting(CallbackDelegate, callback);
        }
    }
}