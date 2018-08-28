namespace Orcus.Administration.Library.Menu.MenuBase
{
    public class NavigationalEntry<TCommandEntry> : CommandCollection<TCommandEntry>, IVisibleMenuItem where TCommandEntry : ICommandMenuEntry
    {
        public object Header { get; set; }
        public object Icon { get; set; }
    }
}