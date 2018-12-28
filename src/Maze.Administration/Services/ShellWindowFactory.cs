using Maze.Administration.Library.Services;
using Maze.Administration.Library.Views;
using Maze.Administration.Views;

namespace Maze.Administration.Services
{
    public class ShellWindowFactory : IShellWindowFactory
    {
        public IShellWindow Create() => new ShellWindow();
    }
}