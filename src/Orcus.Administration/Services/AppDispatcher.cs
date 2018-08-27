using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NuGet.Common;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.Services;

namespace Orcus.Administration.Services
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