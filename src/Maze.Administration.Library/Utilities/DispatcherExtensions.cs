using System;
using System.Windows.Threading;

namespace Maze.Administration.Library.Utilities
{
    /// <summary>
    ///     Provide extensions for the <see cref="Dispatcher"/>
    /// </summary>
    public static class DispatcherExtensions
    {
        /// <summary>
        ///     Invoke the given action if an invoke is required. Else just execute the action in the calling thread.
        /// </summary>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> that should be in sync with the <see cref="action"/></param>
        /// <param name="action">The action that should be executed on the dispatcher thread</param>
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else dispatcher.Invoke(action);
        }
    }
}