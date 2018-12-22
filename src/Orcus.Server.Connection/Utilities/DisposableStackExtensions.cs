using System;
using System.Collections.Generic;
using System.Linq;

namespace Orcus.Server.Connection.Utilities
{
    public static class DisposableStackExtensions
    {
        public static void DisposeWith(this IDisposable disposable, Stack<IDisposable> disposables)
        {
            disposables.Push(disposable);
        }

        public static void Dispose(this Stack<IDisposable> disposables)
        {
            while (disposables.Any()) disposables.Pop().Dispose();
        }
    }
}