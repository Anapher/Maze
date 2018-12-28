using System.Threading;

namespace Maze.Client.Library.Services
{
    /// <summary>
    ///     The synchronization context of the core app thread (STA)
    /// </summary>
    public interface IStaSynchronizationContext
    {
        /// <summary>
        ///     Retrive the current synchronization context of the application thread
        /// </summary>
        SynchronizationContext Current { get; }
    }
}