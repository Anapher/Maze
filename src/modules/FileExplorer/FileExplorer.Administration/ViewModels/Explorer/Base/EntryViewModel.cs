using System.Windows.Media;
using FileExplorer.Shared.Dtos;
using Prism.Mvvm;

namespace FileExplorer.Administration.ViewModels.Explorer.Base
{
    public abstract class EntryViewModel : BindableBase
    {
        public abstract string Label { get; }
        public abstract string Name { get; set; }
        public abstract string SortName { get; }
        public abstract FileExplorerEntry Source { get; }
        public abstract bool IsDirectory { get; }
        public abstract EntryViewModelType Type { get; }
        public abstract ImageSource Icon { get; }
        public abstract string Description { get; }
        public abstract long Size { get; }
    }
}