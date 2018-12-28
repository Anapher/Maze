using System.Threading.Tasks;
using System.Windows.Threading;

namespace Maze.Administration.Library.Services
{
    /// <summary>
    ///     Provides the current application dispatcher for UI operationgs
    /// </summary>
    public interface IAppDispatcher
    {
        /// <summary>
        ///     The <see cref="Dispatcher"/> of the UI thread
        /// </summary>
        Dispatcher Current { get; }

        /// <summary>
        ///     Get the task scheduler of the UI <see cref="Dispatcher"/>
        /// </summary>
        /// <returns>Return the task scheduler.</returns>
        ValueTask<TaskScheduler> GetTaskScheduler();
    }
}