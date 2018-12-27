using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Orcus.Administration.Library.Extensions
{
    public static class DispatcherExtensions
    {
        //https://stackoverflow.com/a/10417542/4166138
        public static Task<TaskScheduler> ToTaskSchedulerAsync(this Dispatcher dispatcher,
            DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var taskCompletionSource = new TaskCompletionSource<TaskScheduler>();
            var invocation =
                dispatcher.BeginInvoke(
                    new Action(() => taskCompletionSource.SetResult(TaskScheduler.FromCurrentSynchronizationContext())),
                    priority);

            invocation.Aborted += (s, e) => taskCompletionSource.SetCanceled();

            return taskCompletionSource.Task;
        }
    }
}