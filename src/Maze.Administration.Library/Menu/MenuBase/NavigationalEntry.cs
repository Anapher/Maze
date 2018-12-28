namespace Maze.Administration.Library.Menu.MenuBase
{
    public class NavigationalEntry<TCommandEntry> : CommandCollection<TCommandEntry>, IVisibleMenuItem where TCommandEntry : ICommandMenuEntry
    {
        public NavigationalEntry(bool isOrdered)
        {
            IsOrdered = isOrdered;
        }

        public NavigationalEntry()
        {
        }

        public object Header { get; set; }
        public object Icon { get; set; }
    }
}