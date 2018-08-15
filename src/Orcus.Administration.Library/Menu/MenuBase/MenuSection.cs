using System.Collections.ObjectModel;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public class MenuSection<TItem> : Collection<IMenuEntry<TItem>>, IMenuEntry<TItem>
    {
    }
}