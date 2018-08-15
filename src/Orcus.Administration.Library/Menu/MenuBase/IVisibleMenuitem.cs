namespace Orcus.Administration.Library.Menu.MenuBase
{
    public interface IVisibleMenuitem<in TItem> : IMenuEntry<TItem>
    {
        object Header { get; }
        object Icon { get; }
    }
}