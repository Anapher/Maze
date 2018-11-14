using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.StatusBar;
using RegistryEditor.Administration.Extensions;
using RegistryEditor.Administration.Model;
using RegistryEditor.Administration.ViewModels.Helpers;
using TreeViewEx.Controls;
using TreeViewEx.Helpers;
using TreeViewEx.Helpers.Selectors;
using TreeViewEx.Helpers.Selectors.Lookup;
using TreeViewEx.Helpers.Selectors.Processors;

namespace RegistryEditor.Administration.ViewModels
{
    public class RegistryTreeViewModel : ISupportTreeSelector<RegistryKeyViewModel, IntegratedRegistryKey>, IAsyncAutoComplete
    {
        private readonly ITargetedRestClient _restClient;
        private readonly IShellStatusBar _statusBar;

        public RegistryTreeViewModel(ITargetedRestClient restClient, IShellStatusBar statusBar)
        {
            _restClient = restClient;
            _statusBar = statusBar;
            Entries = new EntriesHelper<RegistryKeyViewModel>();
            Selection = new TreeRootSelector<RegistryKeyViewModel, IntegratedRegistryKey>(Entries) {Comparers = new[] {new RegistryPathComparer()}};

            Entries.SetEntries(new[]
            {
                RegistryHive.ClassesRoot, RegistryHive.CurrentUser, RegistryHive.LocalMachine, RegistryHive.Users, RegistryHive.CurrentUser
            }.Select(CreateHiveViewModel));
        }

        public IEntriesHelper<RegistryKeyViewModel> Entries { get; set; }
        public ITreeSelector<RegistryKeyViewModel, IntegratedRegistryKey> Selection { get; set; }

        public async Task SelectAsync(IntegratedRegistryKey value)
        {
            await Selection.LookupAsync(value,
                RecrusiveSearch<RegistryKeyViewModel, IntegratedRegistryKey>.LoadSubentriesIfNotLoaded,
                SetSelected<RegistryKeyViewModel, IntegratedRegistryKey>.WhenSelected,
                SetExpanded<RegistryKeyViewModel, IntegratedRegistryKey>.WhenChildSelected);
        }

        private RegistryKeyViewModel CreateHiveViewModel(RegistryHive registryHive)
        {
            var path = registryHive.ToPath();
            var registryKey = new IntegratedRegistryKey {HasSubKeys = true, Name = path, Path = path};
            return new RegistryKeyViewModel(this, registryKey, _restClient, _statusBar, parentViewModel: null);
        }

        public Task<IEnumerable> GetAutoCompleteEntries() => Task.FromResult<IEnumerable>(Entries.All);
    }
}