using Maze.Administration.Library.Menu.MenuBase;

namespace Maze.Administration.Library.Menu.Internal
{
    internal class CommandWrapper<TCommandEntryEntry> : IMenuEntry<TCommandEntryEntry>, IVisibleMenuItem where TCommandEntryEntry : ICommandMenuEntry
    {
        public CommandWrapper(TCommandEntryEntry commandEntry)
        {
            CommandEntry = commandEntry;
        }

        public TCommandEntryEntry CommandEntry { get; set; }

        public object Header => CommandEntry.Header;
        public object Icon => CommandEntry.Icon;
    }
}