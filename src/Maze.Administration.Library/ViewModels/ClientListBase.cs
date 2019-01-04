using MahApps.Metro.IconPacks;

namespace Maze.Administration.Library.ViewModels
{
    public abstract class ClientListBase : ViewModelBase
    {
        private string _searchText;

        protected ClientListBase(string name, PackIconFontAwesomeKind icon)
        {
            Name = name;
            Icon = icon;
        }

        public string Name { get; }
        public PackIconFontAwesomeKind Icon { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value)) OnSearchTextChanged(value);
            }
        }

        protected virtual void OnSearchTextChanged(string searchText)
        {
        }
    }
}