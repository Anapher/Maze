using System;
using System.Windows.Threading;

namespace Orcus.Administration.Library.Utilities
{
    public static class DispatcherExtensions
    {
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else dispatcher.Invoke(action);
        }
    }
}