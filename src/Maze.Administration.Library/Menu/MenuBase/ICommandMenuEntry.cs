using System.Windows.Input;

namespace Orcus.Administration.Library.Menu.MenuBase
{
    public interface ICommandMenuEntry : IVisibleMenuItem
    {
        ICommand Command { get; }
    }
}