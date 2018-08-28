using System.Collections.ObjectModel;
using Orcus.Administration.Library.Menu.Internal;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public abstract class CommandCollection<TCommandEntry> : Collection<IMenuEntry<TCommandEntry>>, IMenuEntry<TCommandEntry>
        where TCommandEntry : ICommandMenuEntry
    {
        public void Add(TCommandEntry command)
        {
            Add(new CommandWrapper<TCommandEntry>(command));
        }
    }
}