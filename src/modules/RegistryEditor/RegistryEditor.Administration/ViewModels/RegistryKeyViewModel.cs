using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Extensions;
using Orcus.Administration.Library.StatusBar;
using Prism.Commands;
using Prism.Mvvm;
using RegistryEditor.Administration.Model;
using RegistryEditor.Administration.Rest;
using TreeViewEx.Controls;
using TreeViewEx.Controls.Models;
using TreeViewEx.Helpers;
using TreeViewEx.Helpers.Selectors;
using Unclassified.TxLib;

namespace RegistryEditor.Administration.ViewModels
{
    public class RegistryKeyViewModel : BindableBase, ISupportTreeSelector<RegistryKeyViewModel, IntegratedRegistryKey>, IAsyncAutoComplete
    {
        private readonly ITargetedRestClient _restClient;
        private readonly IShellStatusBar _statusBar;
        private readonly RegistryTreeViewModel _rootTreeViewModel;
        private int _bringIntoViewToken;

        private DelegateCommand _copyPathCommand;
        private DelegateCommand _refreshCommand;
        private DelegateCommand _toggleExpansionCommand;

        public RegistryKeyViewModel(RegistryTreeViewModel rootTreeViewModel, IntegratedRegistryKey registryKey, ITargetedRestClient restClient,
            IShellStatusBar statusBar, RegistryKeyViewModel parentViewModel)
        {
            RegistryKey = registryKey;
            _rootTreeViewModel = rootTreeViewModel;
            _restClient = restClient;
            _statusBar = statusBar;
            Parent = parentViewModel;

            Entries = new EntriesHelper<RegistryKeyViewModel>(LoadSubKeys);
            Selection = new TreeSelector<RegistryKeyViewModel, IntegratedRegistryKey>(registryKey, this,
                parentViewModel == null ? rootTreeViewModel.Selection : parentViewModel.Selection, Entries);

            if (!registryKey.HasSubKeys)
                Entries.SetEntries(Enumerable.Empty<RegistryKeyViewModel>());
        }

        public RegistryKeyViewModel Parent { get; }
        public IntegratedRegistryKey RegistryKey { get; }
        public bool IsRegistryHive => Parent == null;

        public int BringIntoViewToken
        {
            get => _bringIntoViewToken;
            set => SetProperty(ref _bringIntoViewToken, value);
        }

        public DelegateCommand ToggleExpansionCommand
        {
            get { return _toggleExpansionCommand ?? (_toggleExpansionCommand = new DelegateCommand(() => Entries.IsExpanded = !Entries.IsExpanded)); }
        }

        public DelegateCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new DelegateCommand(async () =>
                {
                    var entries = await LoadSubKeys();
                    Entries.UpdateEntries(entries);
                }));
            }
        }

        public DelegateCommand CopyPathCommand
        {
            get
            {
                return _copyPathCommand ?? (_copyPathCommand = new DelegateCommand(() =>
                {
                    Clipboard.SetText(RegistryKey.Path);
                    _statusBar.ShowStatus(Tx.T("RegistryEditor:StatusBar.PathCopiedToClipboard"));
                }));
            }
        }

        public IEntriesHelper<RegistryKeyViewModel> Entries { get; set; }
        public ITreeSelector<RegistryKeyViewModel, IntegratedRegistryKey> Selection { get; set; }

        public void BringIntoView()
        {
            BringIntoViewToken++;
        }

        private async Task<IEnumerable<RegistryKeyViewModel>> LoadSubKeys()
        {
            var result = await RegistryEditorResource.QuerySubKeys(RegistryKey.Path, _restClient).DisplayOnStatusBarCatchErrors(_statusBar,
                Tx.T("RegistryEditor:StatusBar.LoadSubKeys", "path", RegistryKey.Path), StatusBarAnimation.Search);

            if (result.Failed)
                return Enumerable.Empty<RegistryKeyViewModel>();

            return result.Result.Select(x => new IntegratedRegistryKey {Name = x.Name, HasSubKeys = x.HasSubKeys, Path = RegistryKey.Path + "\\" + x.Name})
                .Select(x => new RegistryKeyViewModel(_rootTreeViewModel, x, _restClient, _statusBar, this));
        }

        protected bool Equals(RegistryKeyViewModel other) => RegistryKey.Equals(other.RegistryKey);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RegistryKeyViewModel) obj);
        }

        public override int GetHashCode() => RegistryKey.GetHashCode();

        public async Task<IEnumerable> GetAutoCompleteEntries()
        {
            if (!Entries.IsLoaded)
                return await Entries.LoadAsync(UpdateMode.Replace, false, null, await Application.Current.Dispatcher.ToTaskSchedulerAsync());

            return Entries.All;
        }
    }
}