using System.Windows.Input;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public interface IItemCommandMenuEntry : ICommandMenuEntry
    {
        ICommand SingleItemCommand { get; }
        ICommand MultipleItemsCommand { get; }
    }
}