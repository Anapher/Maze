using System;

namespace Maze.Administration.Library.StatusBar
{
    /// <summary>
    ///     A base class for a status on a <see cref="IShellStatusBar"/>
    /// </summary>
    public abstract class StatusMessage : IDisposable
    {
        /// <summary>
        ///     The status bar mode
        /// </summary>
        public StatusBarMode StatusBarMode { get; set; } = StatusBarMode.Default;

        /// <summary>
        ///     Dispose the status so it will disappear from the status bar
        /// </summary>
        public virtual void Dispose()
        {
            OnDisposed();
        }

        /// <summary>
        ///     The event that is invoked once the status is disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        ///     Invoke the <see cref="Disposed"/> event
        /// </summary>
        protected virtual void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}