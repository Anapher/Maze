using System.Collections.Generic;
using System.Windows;
using Orcus.Administration.Library.Menu.MenuBase;

namespace Orcus.Administration.Library.Menu
{
    public interface IMenuFactory
    {
        IEnumerable<UIElement> Create<TCommand>(IEnumerable<IMenuEntry<TCommand>> menuEntries, object context)
            where TCommand : ICommandMenuEntry;
    }
}