using System;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Administration.Library.Exceptions;
using Orcus.Administration.Library.Extensions;
using Orcus.Utilities;

namespace Orcus.Administration.Library.StatusBar
{
    /// <summary>
    ///     Extensions for <see cref="IShellStatusBar"/>
    /// </summary>
    public static class ShellStatusBarExtensions
    {
        private const int DefaultMessageTimeout = 5;

        /// <summary>
        ///     Show a text message on the status bar that will automatically disappear 
        /// </summary>
        /// <param name="shellStatusBar">The status bar</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="seconds">The time in seconds the message should stay on the status bar</param>
        /// <param name="cancellationToken">The cancellation token that will immediately remove the status.</param>
        /// <exception cref="TimeoutException">The <see cref="TimeoutException"/> is thrown when the cancellation token is triggered</exception>
        public static async void ShowMessage(this IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None, int? seconds = null,
            CancellationToken cancellationToken = default)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
            {
                await Task.Delay((seconds ?? DefaultMessageTimeout) * 1000, cancellationToken);
            }
        }

        /// <summary>
        ///     Show a text message on the status bar that must be disposed to disappear
        /// </summary>
        /// <param name="shellStatusBar">The status bar.</param>
        /// <param name="message">The text message that should be displayed.</param>
        /// <param name="animation">The animation that should be shown.</param>
        /// <returns></returns>
        public static IDisposable ShowStatus(this IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            return shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation});
        }

        /// <summary>
        ///     Display a success message on the status bar.
        /// </summary>
        /// <param name="shellStatusBar">The status bar</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="seconds">The time in seconds the message should stay on the status bar</param>
        /// <param name="cancellationToken">The cancellation token that will immediately remove the status.</param>
        /// <exception cref="TimeoutException">The <see cref="TimeoutException"/> is thrown when the cancellation token is triggered</exception>
        public static async void ShowSuccess(this IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None, int? seconds = null,
            CancellationToken cancellationToken = default)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message)
            {
                Animation = animation,
                StatusBarMode = StatusBarMode.Success
            }))
            {
                await Task.Delay((seconds ?? DefaultMessageTimeout) * 1000, cancellationToken);
            }
        }

        /// <summary>
        ///     Display an error message on the status bar.
        /// </summary>
        /// <param name="shellStatusBar">The status bar</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="seconds">The time in seconds the message should stay on the status bar</param>
        /// <param name="cancellationToken">The cancellation token that will immediately remove the status.</param>
        /// <exception cref="TimeoutException">The <see cref="TimeoutException"/> is thrown when the cancellation token is triggered</exception>
        public static async void ShowError(this IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None, int? seconds = null,
            CancellationToken cancellationToken = default)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message)
            {
                Animation = animation,
                StatusBarMode = StatusBarMode.Error
            }))
            {
                await Task.Delay((seconds ?? DefaultMessageTimeout) * 1000, cancellationToken);
            }
        }

        /// <summary>
        ///     Display a warning message on the status bar.
        /// </summary>
        /// <param name="shellStatusBar">The status bar</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="seconds">The time in seconds the message should stay on the status bar</param>
        /// <param name="cancellationToken">The cancellation token that will immediately remove the status.</param>
        /// <exception cref="TimeoutException">The <see cref="TimeoutException"/> is thrown when the cancellation token is triggered</exception>
        public static async void ShowWarning(this IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None, int? seconds = null,
            CancellationToken cancellationToken = default)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message)
            {
                Animation = animation,
                StatusBarMode = StatusBarMode.Warning
            }))
            {
                await Task.Delay((seconds ?? DefaultMessageTimeout) * 1000, cancellationToken);
            }
        }

        /// <summary>
        ///     Show a text message on the status bar while a <see cref="Task"/> is awaited.
        /// </summary>
        /// <param name="shellStatusBar">The status bar</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="task">The task that should be awaited while the status should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <returns>Return the task that wraps the <see cref="task"/> with the creation of the status.</returns>
        public static async Task ShowMessage(this IShellStatusBar shellStatusBar, string message, Task task,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
            {
                await task;
            }
        }

        /// <summary>
        ///     Show a text message on the status bar while a <see cref="Task{T}"/> is awaited.
        /// </summary>
        /// <param name="shellStatusBar">The status bar</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="task">The task that should be awaited while the status should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <returns>Return the task that wraps the <see cref="task"/> with the creation of the status.</returns>
        public static async Task<T> ShowMessage<T>(this IShellStatusBar shellStatusBar, string message, Task<T> task,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) { Animation = animation }))
            {
                return await task;
            }
        }

        /// <summary>
        ///     Display a status on a <see cref="IShellStatusBar"/> while a <see cref="Task"/> is running
        /// </summary>
        /// <param name="task">The task that should be visualized on the status bar.</param>
        /// <param name="shellStatusBar">The status bar.</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <returns>Return the task that wraps the <see cref="task"/> with the creation of the status.</returns>
        //public static async Task DisplayOnStatusBar(this Task task, IShellStatusBar shellStatusBar, string message,
        //    StatusBarAnimation animation = StatusBarAnimation.None)
        //{
        //    using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
        //    {
        //        await task;
        //    }
        //}

        /// <summary>
        ///     Display a status on a <see cref="IShellStatusBar"/> while a <see cref="Task{}"/> is running. To avoid flickering in case the task executes very fast, the message isn't shown immediately and will be visible for at least one second.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="task">The task that should be visualized on the status bar.</param>
        /// <param name="shellStatusBar">The status bar.</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <returns>Return the task that wraps the <see cref="task"/> with the creation of the status.</returns>
        public static async Task<T> DisplayOnStatusBar<T>(this Task<T> task, IShellStatusBar shellStatusBar,
            string message, StatusBarAnimation animation = StatusBarAnimation.None, bool mustShow = false)
        {
            await DisplayOnStatusBar((Task) task, shellStatusBar, message, animation, mustShow);
            return task.Result;
        }

        /// <summary>
        ///     Display a status on a <see cref="IShellStatusBar"/> while a <see cref="Task"/> is running. To avoid flickering in case the task executes very fast, the message isn't shown immediately and will be visible for at least one second.
        /// </summary>
        /// <param name="task">The task that should be visualized on the status bar.</param>
        /// <param name="shellStatusBar">The status bar.</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="mustShow">If true, the status will be shown on the status bar even if the task finishes extremly fast.</param>
        /// <returns>Return the task that wraps the <see cref="task"/> with the creation of the status.</returns>
        public static async Task DisplayOnStatusBar(this Task task, IShellStatusBar shellStatusBar,
            string message, StatusBarAnimation animation = StatusBarAnimation.None, bool mustShow = false)
        {
            //if the task executes synchronously, don't display anything to avoid "flickering"
            if (!mustShow)
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(200));
                if (completedTask == task)
                    return;
            }

            var beginTime = DateTimeOffset.UtcNow;
            var status = shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation});

            try
            {
                await task;
            }
            catch (Exception)
            {
                status.Dispose();
                throw;
            }

            //the status should be displayed at least one second to avoid flickering. Even if the task finished,
            //continue displaying the status for the remaining of the time

            var diff = TimeSpan.FromSeconds(1) - (DateTimeOffset.UtcNow - beginTime);
            if (diff < TimeSpan.Zero)
                status.Dispose();
            else Task.Delay(diff).ContinueWith(_ => status.Dispose()).Forget();
        }

        /// <summary>
        ///     Display a status on a <see cref="IShellStatusBar"/> while a <see cref="Task"/> is running. Any exception will be caught and displayed on the status bar. To avoid flickering in case the task executes very fast, the message isn't shown immediately and will be visible for at least one second.
        /// </summary>
        /// <param name="task">The task that should be visualized on the status bar.</param>
        /// <param name="shellStatusBar">The status bar.</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="mustShow">If true, the status will be shown on the status bar even if the task finishes extremly fast.</param>
        /// <returns>Return the task that wraps the <see cref="task"/> with the creation of the status and the result.</returns>
        public static async Task<SuccessOrError<T>> DisplayOnStatusBarCatchErrors<T>(this Task<T> task,
            IShellStatusBar shellStatusBar, string message, StatusBarAnimation animation = StatusBarAnimation.None, bool mustShow = false)
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
        ///     Display a status on a <see cref="IShellStatusBar"/> while a <see cref="Task"/> is running. Any exception will be caught and displayed on the status bar. To avoid flickering in case the task executes very fast, the message isn't shown immediately and will be visible for at least one second.
        /// </summary>
        /// <param name="task">The task that should be visualized on the status bar.</param>
        /// <param name="shellStatusBar">The status bar.</param>
        /// <param name="message">The text message that should be displayed</param>
        /// <param name="animation">The animation that should be shown</param>
        /// <param name="mustShow">If true, the status will be shown on the status bar even if the task finishes extremly fast.</param>
        /// <returns>Return the task that wraps the <see cref="task"/> with the creation of the status and whether an exception was not thrown (return <code>false</code> if an exception was thrown).</returns>
        public static async Task<bool> DisplayOnStatusBarCatchErrors(this Task task, IShellStatusBar shellStatusBar,
            string message, StatusBarAnimation animation = StatusBarAnimation.None, bool mustShow = false)
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