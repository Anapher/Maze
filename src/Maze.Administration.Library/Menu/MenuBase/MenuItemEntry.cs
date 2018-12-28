using System.Windows.Controls;

namespace Maze.Administration.Library.Menu.MenuBase
{
    public class MenuItemEntry<TCommandEntry> : IMenuEntry<TCommandEntry> where TCommandEntry : ICommandMenuEntry
    {
        public MenuItemEntry(MenuItem menuItem)
        {
            MenuItem = menuItem;
        }

        public MenuItem MenuItem { get; }

        public bool VisibleForSingle { get; set; } = true;
        public bool VisibleForMultiple { get; set; } = true;
    }
}