using System;
using System.Threading.Tasks;
using Anapher.Wpf.Toolkit.StatusBar;
using Maze.Administration.Library.Exceptions;

namespace Maze.Administration.Library.Extensions
{
    public static class StatusBarExtensions
    {
        /// <summary>
        ///     Display a status on a <see cref="IShellStatusBar" /> while a <see cref="Task" /> is running. Any exception will be
        ///     caught and displayed on the status bar. To avoid flickering in case the task executes very fast, the message isn't
        ///     shown immediately and will be visible for at least one second.
        /// </summary>
        /// <param name="task">The task that should be visualized on the status bar.</param>
        /// <param name="shellStatusBar">The status bar.</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="mustShow">If true, the status will be shown on the status bar even if the task finishes extremly fast.</param>
        /// <returns>Return the task that wraps the <see cref="task" /> with the creation of the status and the result.</returns>
        public static async Task<SuccessOrError<T>> DisplayOnStatusBarCatchErrors<T>(this Task<T> task, IShellStatusBar shellStatusBar,
            string message, StatusBarAnimation animation = StatusBarAnimation.None, bool mustShow = false)
        {
            try
            {
                return await task.DisplayOnStatusBar(shellStatusBar, message, animation, mustShow);
            }
            catch (RestException e)
            {
                shellStatusBar.ShowError(e.GetRestExceptionMessage());
                return SuccessOrError<T>.DefaultFailed;
            }
            catch (Exception e)
            {
                shellStatusBar.ShowError(e.Message);
                return SuccessOrError<T>.DefaultFailed;
            }
        }

        /// <summary>
        ///     Display a status on a <see cref="IShellStatusBar" /> while a <see cref="Task" /> is running. Any exception will be
        ///     caught and displayed on the status bar. To avoid flickering in case the task executes very fast, the message isn't
        ///     shown immediately and will be visible for at least one second.
        /// </summary>
        /// <param name="task">The task that should be visualized on the status bar.</param>
        /// <param name="shellStatusBar">The status bar.</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="mustShow">If true, the status will be shown on the status bar even if the task finishes extremly fast.</param>
        /// <returns>
        ///     Return the task that wraps the <see cref="task" /> with the creation of the status and whether an exception
        ///     was not thrown (return <code>false</code> if an exception was thrown).
        /// </returns>
        public static async Task<bool> DisplayOnStatusBarCatchErrors(this Task task, IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None, bool mustShow = false)
        {
            try
            {
                await task.DisplayOnStatusBar(shellStatusBar, message, animation, mustShow);
                return true;
            }
            catch (RestException e)
            {
                shellStatusBar.ShowError(e.GetRestExceptionMessage());
                return false;
            }
            catch (Exception e)
            {
                shellStatusBar.ShowError(e.Message);
                return false;
            }
        }
    }
}