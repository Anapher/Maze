using Maze.Administration.Library.Views;

namespace Maze.Administration.Library.Services
{
    /// <summary>
    ///     A factory that created <see cref="IShellWindow"/>s.
    /// </summary>
    public interface IShellWindowFactory
    {
        /// <summary>
        ///     Create a new <see cref="IShellWindow"/>
        /// </summary>
        /// <returns>Return the created window.</returns>
        IShellWindow Create();
    }
}