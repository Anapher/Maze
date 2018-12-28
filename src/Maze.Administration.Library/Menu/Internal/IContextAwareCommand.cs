using System.Windows.Input;

namespace Maze.Administration.Library.Menu.Internal
{
    internal interface IContextAwareCommand : ICommand
    {
        object Context { get; set; }
    }
}