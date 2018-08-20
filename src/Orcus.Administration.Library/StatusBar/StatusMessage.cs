using System;

namespace Orcus.Administration.Library.StatusBar
{
    public abstract class StatusMessage : IDisposable
    {
        public StatusBarMode StatusBarMode { get; set; } = StatusBarMode.Default;

        public virtual void Dispose()
        {
            OnDisposed();
        }

        public event EventHandler Disposed;

        protected virtual void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}