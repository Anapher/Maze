using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using FileExplorer.Administration.Controls.Models;
using Nito.AsyncEx;

namespace FileExplorer.Administration.Helpers
{
    public interface ISupportEntriesHelper<TVm>
    {
        IEntriesHelper<TVm> Entries { get; set; }
    }

    /// <summary>
    ///     Helper view model class that provide support of loading sub-entries.
    /// </summary>
    /// <typeparam name="TVm"></typeparam>
    public interface IEntriesHelper<TVm> : INotifyPropertyChanged
    {
        /// <summary>
        ///     Load when expand the first time.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        ///     Whether subentries loaded.
        /// </summary>
        bool IsLoaded { get; set; }

        bool IsLoading { get; set; }

        IEnumerable<TVm> AllNonBindable { get; }

        /// <summary>
        ///     A list of sub-entries, after loaded.
        /// </summary>
        ObservableCollection<TVm> All { get; }

        AsyncLock LoadingLock { get; }

        /// <summary>
        ///     Call to load sub-entries.
        /// </summary>
        /// <param name="force">Load sub-entries even if it's already loaded.</param>
        /// <returns></returns>
        Task<IEnumerable<TVm>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace, bool force = false,
            object parameter = null);

        Task UnloadAsync();

        /// <summary>
        ///     Used to preload sub-entries, fully overwrite entries stored in the helper.
        /// </summary>
        /// <param name="viewModels"></param>
        void SetEntries(UpdateMode updateMode = UpdateMode.Replace, params TVm[] viewModels);

        event EventHandler EntriesChanged;
    }
}