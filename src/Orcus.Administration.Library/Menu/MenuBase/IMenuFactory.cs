using System.Collections.Generic;
using System.Windows;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public interface IMenuFactory
    {
        IEnumerable<UIElement> Create<T>(IEnumerable<IMenuEntry<T>> menuEntries);
    }
}