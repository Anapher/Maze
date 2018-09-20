namespace Orcus.Administration.Library.Menu.MenuBase
{
    public class MenuSection<TCommandEntry> : CommandCollection<TCommandEntry> where TCommandEntry : ICommandMenuEntry
    {
        public MenuSection(bool isOrdered)
        {
            IsOrdered = isOrdered;
        }

        public MenuSection()
        {
        }
    }
}