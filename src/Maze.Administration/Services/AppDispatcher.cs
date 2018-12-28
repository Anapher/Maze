using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NuGet.Common;
using Maze.Administration.Library.Extensions;
using Maze.Administration.Library.Services;

namespace Maze.Administration.Services
{
    public class AppDispatcher : IAppDispatcher
    {
        private readonly AsyncLazy<TaskScheduler> _lazyTaskScheduler;

        public AppDispatcher()
        {
            _lazyTaskScheduler = new AsyncLazy<TaskScheduler>(() => Current.ToTaskSchedulerAsync());
        }

        public Dispatcher Current => Application.Current.Dispatcher;

        public async ValueTask<TaskScheduler> GetTaskScheduler()
        {
            return await _lazyTaskScheduler;
        }
    }
}