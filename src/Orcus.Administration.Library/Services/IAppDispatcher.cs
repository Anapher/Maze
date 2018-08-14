using System.Windows.Threading;

namespace Orcus.Administration.Library.Services
{
    public interface IAppDispatcher
    {
        Dispatcher Current { get; }
    }
}