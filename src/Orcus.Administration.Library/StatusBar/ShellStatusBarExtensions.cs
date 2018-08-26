using System;
using System.Threading;
using System.Threading.Tasks;
using Orcus.Administration.Library.Exceptions;
using Orcus.Administration.Library.Extensions;

namespace Orcus.Administration.Library.StatusBar
{
    public static class ShellStatusBarExtensions
    {
        private const int DefaultMessageTimeout = 5;

        public static async void ShowMessage(this IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None, int? seconds = null,
            CancellationToken cancellationToken = default)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
            {
                await Task.Delay((seconds ?? DefaultMessageTimeout) * 1000, cancellationToken);
            }
        }

        public static IDisposable ShowStatus(this IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            return shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation});
        }

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

        public static async Task ShowMessage(this IShellStatusBar shellStatusBar, string message, Task task,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
            {
                await task;
            }
        }

        public static async Task<T> ShowMessage<T>(this IShellStatusBar shellStatusBar, string message, Task<T> task,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) { Animation = animation }))
            {
                return await task;
            }
        }

        public static async Task DisplayOnStatusBar(this Task task, IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
            {
                await task;
            }
        }

        public static async Task<T> DisplayOnStatusBar<T>(this Task<T> task, IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) { Animation = animation }))
            {
                return await task;
            }
        }

        public static async Task<SuccessOrError<T>> DisplayOnStatusBarCatchErrors<T>(this Task<T> task,
            IShellStatusBar shellStatusBar, string message, StatusBarAnimation animation = StatusBarAnimation.None)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
            {
                try
                {
                    return await task;
                }
                catch (RestException e)
                {
                    shellStatusBar.ShowError(e.GetRestExceptionMessage());
                }
                catch (Exception e)
                {
                    shellStatusBar.ShowError(e.Message);
                }

                return SuccessOrError<T>.DefaultFailed;
            }
        }
    }
}