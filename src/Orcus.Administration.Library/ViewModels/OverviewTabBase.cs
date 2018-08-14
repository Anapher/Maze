using MahApps.Metro.IconPacks;

namespace Orcus.Administration.Library.ViewModels
{
    public class OverviewTabBase : ViewModelBase
    {
        public OverviewTabBase(string title, PackIconFontAwesomeKind icon)
        {
            Title = title;
            Icon = icon;
        }

        public string Title { get; }
        public PackIconFontAwesomeKind Icon { get; }
    }
}