using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orcus.Administration.Modules.Models
{
    /// <summary>
    /// Represents stateful item loader contract that supports pagination and background loading
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IItemLoader<T>
    {
        bool IsMultiSource { get; }

        IItemLoaderState State { get; }

        IEnumerable<T> GetCurrent();

        Task LoadNextAsync(IProgress<IItemLoaderState> progress, CancellationToken cancellationToken);

        Task UpdateStateAsync(IProgress<IItemLoaderState> progress, CancellationToken cancellationToken);

        void Reset();

        Task<int> GetTotalCountAsync(int maxCount, CancellationToken cancellationToken);
    }
}