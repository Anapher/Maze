using System.Collections.ObjectModel;
using Orcus.Administration.Library.Menu.Internal;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public abstract class CommandCollection<TCommandEntry> : Collection<IMenuEntry<TCommandEntry>>, IMenuEntry<TCommandEntry>
        where TCommandEntry : ICommandMenuEntry
    {
        public bool IsOrdered { get; set; }

        public void Add(TCommandEntry command)
        {
            Add(new CommandWrapper<TCommandEntry>(command));
        }

        public void AddAtBeginning(TCommandEntry command)
        {
            Insert(0, new CommandWrapper<TCommandEntry>(command));
        }
    }
}