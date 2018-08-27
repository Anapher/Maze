using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Anapher.Wpf.Swan;
using FileExplorer.Administration.Controls.Models;
using Nito.AsyncEx;
using Orcus.Utilities;

namespace FileExplorer.Administration.Helpers
{
    public class EntriesHelper<TVm> : IEntriesHelper<TVm>, INotifyPropertyChanged
    {
        private readonly AsyncLock _loadingLock = new AsyncLock();
        private bool _clearBeforeLoad;
        private bool _isExpanded;
        //private bool _isLoading = false;
        private bool _isLoaded;
        private bool _isLoading;

        private CancellationTokenSource _lastCancellationToken = new CancellationTokenSource();
        private DateTime _lastRefreshTimeUtc = DateTime.MinValue;
        protected Func<bool, object, Task<IEnumerable<TVm>>> LoadSubEntryFunc;
        private IEnumerable<TVm> _subItemList = new List<TVm>();

        public EntriesHelper(Func<bool, object, Task<IEnumerable<TVm>>> loadSubEntryFunc)
        {
            LoadSubEntryFunc = loadSubEntryFunc;

            All = new TransactionalObservableCollection<TVm>();
            All.Add(default(TVm));
        }

        public EntriesHelper(Func<bool, Task<IEnumerable<TVm>>> loadSubEntryFunc)
            : this((b, __) => loadSubEntryFunc(b))
        {
        }

        public EntriesHelper(Func<Task<IEnumerable<TVm>>> loadSubEntryFunc)
            : this(_ => loadSubEntryFunc())
        {
        }

        public EntriesHelper(params TVm[] entries)
        {
            _isLoaded = true;
            All = new TransactionalObservableCollection<TVm>();
            _subItemList = entries;
            ((TransactionalObservableCollection<TVm>) All).AddRange(entries);
        }

        public bool ClearBeforeLoad
        {
            get => _clearBeforeLoad;
            set => _clearBeforeLoad = value;
        }

        public DateTime LastRefreshTimeUtc => _lastRefreshTimeUtc;

        public ObservableCollection<TVm> All { get; private set; }

        public AsyncLock LoadingLock => _loadingLock;

        public async Task UnloadAsync()
        {
            _lastCancellationToken.Cancel(); //Cancel previous load.                
            using (var releaser = await _loadingLock.LockAsync())
            {
                _subItemList = new List<TVm>();
                All.Clear();
                _isLoaded = false;
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value && !_isExpanded)
                    LoadAsync().Forget();
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoaded
        {
            get => _isLoaded;
            set
            {
                _isLoaded = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler EntriesChanged;


        public IEnumerable<TVm> AllNonBindable => _subItemList;

        public async Task<IEnumerable<TVm>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace, bool force = false,
            object parameter = null, TaskScheduler uiScheduler = null)
        {
            if (LoadSubEntryFunc != null) //Ignore if contructucted using entries but not entries func
            {
                _lastCancellationToken.Cancel(); //Cancel previous load.                
                using (await _loadingLock.LockAsync())
                {
                    _lastCancellationToken = new CancellationTokenSource();
                    if (!_isLoaded || force)
                    {
                        if (_clearBeforeLoad)
                            All.Clear();

                        if (uiScheduler == null)
                            uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                        try
                        {
                            IsLoading = true;
                            await LoadSubEntryFunc(_isLoaded, parameter).ContinueWith((prevTask, _) =>
                            {
                                IsLoaded = true;
                                IsLoading = false;
                                if (!prevTask.IsCanceled && !prevTask.IsFaulted)
                                {
                                    SetEntries(updateMode, prevTask.Result.ToArray());
                                    _lastRefreshTimeUtc = DateTime.UtcNow;
                                }
                            }, _lastCancellationToken, uiScheduler);
                        }
                        catch (InvalidOperationException)
                        {
                        }
                    }
                }
            }
            return _subItemList;
        }


        private void UpdateEntries(params TVm[] viewModels)
        {
            var all = (TransactionalObservableCollection<TVm>) All;
            all.SuspendCollectionChangeNotification();

            var removeItems = all.Where(vm => !viewModels.Contains(vm)).ToList();
            var addItems = viewModels.Where(vm => !all.Contains(vm)).ToList();

            if (addItems.Count == 0 && removeItems.Count == 0)
                return; //nothing to do here

            foreach (var vm in removeItems)
                all.Remove(vm);

            foreach (var vm in addItems)
                all.Add(vm);

            _subItemList = all.ToArray().ToList();
            all.NotifyChanges();

            EntriesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetEntries(UpdateMode updateMode = UpdateMode.Replace, params TVm[] viewModels)
        {
            switch (updateMode)
            {
                case UpdateMode.Update:
                    UpdateEntries(viewModels);
                    break;
                case UpdateMode.Replace:
                    SetEntries(viewModels);
                    break;
                default:
                    throw new NotSupportedException("UpdateMode");
            }
        }

        private void SetEntries(params TVm[] viewModels)
        {
            _subItemList = viewModels.ToList();
            var all = (TransactionalObservableCollection<TVm>) All;
            all.SuspendCollectionChangeNotification();
            all.Clear();
            all.NotifyChanges();
            all.AddRange(viewModels);
            all.NotifyChanges();

            if (EntriesChanged != null)
                EntriesChanged(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}