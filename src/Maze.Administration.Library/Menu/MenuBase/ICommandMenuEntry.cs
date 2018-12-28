using System.Windows.Input;

namespace Maze.Administration.Library.Menu.MenuBase
{
    public interface ICommandMenuEntry : IVisibleMenuItem
    {
        ICommand Command { get; }
    }
}