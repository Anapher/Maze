using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Administration.Library.StatusBar
{
    public static class ShellStatusBarExtensions
    {
        private const int DefaultMessageTimeout = 3;

        public static async void ShowMessage(this IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None, int seconds = DefaultMessageTimeout,
            CancellationToken cancellationToken = default)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
            {
                await Task.Delay(seconds * 1000, cancellationToken);
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

        public static async Task DisplayOnStatusBar(this Task task, IShellStatusBar shellStatusBar, string message,
            StatusBarAnimation animation = StatusBarAnimation.None)
        {
            using (shellStatusBar.PushStatus(new TextStatusMessage(message) {Animation = animation}))
            {
                await task;
            }
        }
    }
}