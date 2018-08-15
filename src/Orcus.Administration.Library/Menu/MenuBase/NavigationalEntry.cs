using System.Collections.ObjectModel;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public class NavigationalEntry<TItem> : Collection<IMenuEntry<TItem>>, IVisibleMenuitem<TItem>
    {
        public object Header { get; set; }
        public object Icon { get; set; }
    }
}