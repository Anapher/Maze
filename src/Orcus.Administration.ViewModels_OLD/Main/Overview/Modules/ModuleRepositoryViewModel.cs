using System;
using Unclassified.TxLib;

namespace Orcus.Administration.ViewModels.Main.Overview.Modules.Feed
{
    public class ModuleRepositoryViewModel
    {
        public Uri SourceUri { get; }
        public string DisplayText => IsAll ? Tx.T("All") : SourceUri.Host;

        public ModuleRepositoryViewModel(Uri sourceUri)
        {
            SourceUri = sourceUri;
        }

        private ModuleRepositoryViewModel()
        {
        }

        public static ModuleRepositoryViewModel All { get; } = new ModuleRepositoryViewModel();

        public bool IsAll => this == All;
    }
}