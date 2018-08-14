using System.Windows;
using System.Windows.Threading;
using Orcus.Administration.Library.Services;

namespace Orcus.Administration.Services
{
    public class AppDispatcher : IAppDispatcher
    {
        public Dispatcher Current => Application.Current.Dispatcher;
    }
}