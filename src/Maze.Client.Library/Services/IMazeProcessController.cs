namespace Maze.Client.Library.Services
{
    /// <summary>
    ///     Controller methods of the core process of Maze
    /// </summary>
    public interface IMazeProcessController
    {
        /// <summary>
        ///     Shutdown the Maze process
        /// </summary>
        void Shutdown();

        /// <summary>
        ///     Restart the Maze process
        /// </summary>
        void Restart();
    }
}