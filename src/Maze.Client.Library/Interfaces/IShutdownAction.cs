using Maze.Modules.Api;

namespace Maze.Client.Library.Interfaces
{
    /// <summary>
    ///     Triggered when the application shuts down
    /// </summary>
    public interface IShutdownAction : IActionInterface<ShutdownContext>
    {
    }

    /// <summary>
    ///     Context of the <see cref="IShutdownAction" />
    /// </summary>
    public class ShutdownContext
    {
        public ShutdownContext(ShutdownTrigger shutdownTrigger)
        {
            ShutdownTrigger = shutdownTrigger;
        }

        /// <summary>
        ///     The trigger of the shutdown
        /// </summary>
        public ShutdownTrigger ShutdownTrigger { get; }
    }

    /// <summary>
    ///     The trigger of the shutdown
    /// </summary>
    public enum ShutdownTrigger
    {
        /// <summary>
        ///     The system is shutting down
        /// </summary>
        SystemShutdown,

        /// <summary>
        ///     The main thread is shutting down
        /// </summary>
        MainThreadShutdown,

        /// <summary>
        ///     The application wants to shutdown
        /// </summary>
        ApplicationShutdown,

        /// <summary>
        ///     The application wants to restart
        /// </summary>
        ApplicationRestart
    }
}