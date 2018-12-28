using System.Collections.Generic;
using System.Windows;
using Maze.Administration.Library.Menu.MenuBase;

namespace Maze.Administration.Library.Menu
{
    public interface IMenuFactory
    {
        IEnumerable<UIElement> Create<TCommand>(IEnumerable<IMenuEntry<TCommand>> menuEntries, object context)
            where TCommand : ICommandMenuEntry;
    }
}