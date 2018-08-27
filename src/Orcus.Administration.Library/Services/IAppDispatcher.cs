using System.Threading.Tasks;
using System.Windows.Threading;

namespace Orcus.Administration.Library.Services
{
    public interface IAppDispatcher
    {
        Dispatcher Current { get; }
        ValueTask<TaskScheduler> GetTaskScheduler();
    }
}