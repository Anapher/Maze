using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FileExplorer.Administration.Controls.Models;
using Orcus.Utilities;

namespace FileExplorer.Administration.Helpers
{
    public class TreeRootSelector<VM, T> : TreeSelector<VM, T>, ITreeRootSelector<VM, T>
    {
        private Stack<ITreeSelector<VM, T>> _prevPath;
        private ObservableCollection<VM> _rootItems;

        private T _selectedValue;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="entryHelper"></param>
        /// <param name="compareFunc"></param>
        /// <param name="rootLevel">
        ///     Level of TreeItem to consider as root, root items should shown in expander
        ///     (e.g. in OverflowedAndRootItems) and have caption and expander hidden when the path is longer than it.
        /// </param>
        public TreeRootSelector(IEntriesHelper<VM> entryHelper) // int rootLevel = 0,
            //params Func<T, T, HierarchicalResult>[] compareFuncs)
            : base(entryHelper)
        {
            //_rootLevel = rootLevel;
            //_compareFuncs = compareFuncs;
            //Comparers = new [] { PathComparer.LocalDefault };
        }

        //private int _rootLevel;

        public ObservableCollection<VM> OverflowedAndRootItems
        {
            get
            {
                if (_rootItems == null) updateRootItems();
                return _rootItems;
            }
            set
            {
                _rootItems = value;
                OnPropertyChanged();
            }
        }


        //public int RootLevel { get { return _rootLevel; } set { _rootLevel = value; } }

        public IEnumerable<ICompareHierarchy<T>> Comparers { get; set; }

        public override void ReportChildSelected(Stack<ITreeSelector<VM, T>> path)
        {
            var _prevSelector = SelectedSelector;
            var _prevSelectedValue = _selectedValue;
            _prevPath = path;

            SelectedSelector = path.Last();
            _selectedValue = path.Last().Value;
            if (_prevSelectedValue != null && !_prevSelectedValue.Equals(path.Last().Value))
                _prevSelector.IsSelected = false;
            OnPropertyChanged(nameof(SelectedValue));
            OnPropertyChanged(nameof(SelectedViewModel));
            SelectionChanged?.Invoke(this, EventArgs.Empty);

            //WARNING: Commented out by Sorzus. I don't understand why you have to load all the root items if a child gets selected
            //updateRootItems(path);
        }

        public override void ReportChildDeselected(Stack<ITreeSelector<VM, T>> path)
        {
        }

        public async Task SelectAsync(T value)
        {
            if (_selectedValue == null || CompareHierarchy(_selectedValue, value) != HierarchicalResult.Current)
                await LookupAsync(value, RecrusiveSearch<VM, T>.LoadSubentriesIfNotLoaded,
                    SetSelected<VM, T>.WhenSelected, SetChildSelected<VM, T>.ToSelectedChild);
        }

        public event EventHandler SelectionChanged;

        public ITreeSelector<VM, T> SelectedSelector { get; private set; }

        public VM SelectedViewModel => SelectedSelector == null ? default : SelectedSelector.ViewModel;

        public T SelectedValue
        {
            get => _selectedValue;
            set => SelectAsync(value).Forget();
        }

        public HierarchicalResult CompareHierarchy(T value1, T value2)
        {
            foreach (var c in Comparers)
            {
                var retVal = c.CompareHierarchy(value1, value2);
                if (retVal != HierarchicalResult.Unrelated)
                    return retVal;
            }

            return HierarchicalResult.Unrelated;
        }

        private async Task updateRootItemsAsync(ITreeSelector<VM, T> selector, ObservableCollection<VM> rootItems,
            int level)
        {
            if (level == 0)
                return;

            var rootTreeSelectors = new List<ITreeSelector<VM, T>>();
            await selector.LookupAsync(default, BroadcastNextLevel<VM, T>.LoadSubentriesIfNotLoaded,
                new TreeLookupProcessor<VM, T>(HierarchicalResult.All, (hr, p, c) =>
                {
                    rootTreeSelectors.Add(c);
                    return true;
                }));

            foreach (var c in rootTreeSelectors)
            {
                rootItems.Add(c.ViewModel);
                c.IsRoot = true;
                await updateRootItemsAsync(c, rootItems, level - 1);
            }
        }

        private void updateRootItems(Stack<ITreeSelector<VM, T>> path = null)
        {
            if (_rootItems == null)
                _rootItems = new ObservableCollection<VM>();
            else _rootItems.Clear();
            if (path != null && path.Count > 0)
            {
                foreach (var p in path.Reverse())
                    if (!EntryHelper.AllNonBindable.Contains(p.ViewModel))
                        _rootItems.Add(p.ViewModel);
                _rootItems.Add(default); //Separator
            }

            updateRootItemsAsync(this, _rootItems, 2).Forget();
        }
    }
}