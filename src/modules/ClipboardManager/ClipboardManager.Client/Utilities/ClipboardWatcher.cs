using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClipboardManager.Shared.Native;
using Orcus.Client.Library.Services;

namespace ClipboardManager.Client.Utilities
{
    /// <summary>
    ///     Provides notifications when the contents of the clipboard is updated. This class is thread safe.
    /// </summary>
    public class ClipboardWatcher
    {
        private static readonly object EventLock = new object();
        private readonly IStaSynchronizationContext _synchronizationContext;
        private EventHandler<IDataObject> _clipboardChangeEventHandler;
        private NotificationForm _form;

        public ClipboardWatcher(IStaSynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        /// <summary>
        ///     Occurs when the contents of the clipboard is updated.
        /// </summary>
        public event EventHandler<IDataObject> ClipboardUpdated
        {
            add
            {
                lock (EventLock)
                {
                    if (_clipboardChangeEventHandler == null)
                    {
                        _synchronizationContext.Current.Send(state =>
                        {
                            var form = new NotificationForm();
                            form.Show();

                            _form = form;
                        }, null);

                        _form.ClipboardUpdated += FormOnClipboardUpdated;
                    }

                    _clipboardChangeEventHandler += value;
                }
            }
            remove
            {
                lock (EventLock)
                {
                    _clipboardChangeEventHandler -= value;
                    if (_clipboardChangeEventHandler == null)
                    {
                        _form.Invoke(new Action(() => _form.Close()));
                        _form.Dispose();
                        _form = null;
                    }
                }
            }
        }

        private void FormOnClipboardUpdated(object sender, EventArgs e)
        {
            var clipboardData = Clipboard.GetDataObject(); //we are on the STA thread here

            var handler = _clipboardChangeEventHandler; //important for thread safety
            Task.Run(() => handler?.Invoke(this, clipboardData));
        }

        /// <summary>
        ///     Hidden form to receive the WM_CLIPBOARDUPDATE message.
        /// </summary>
        private class NotificationForm : Form
        {
            private static readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

            public NotificationForm()
            {
                NativeMethods.SetParent(Handle, HWND_MESSAGE);
                NativeMethods.AddClipboardFormatListener(Handle);
            }

            public event EventHandler ClipboardUpdated;

            protected override void SetVisibleCore(bool value)
            {
                base.SetVisibleCore(false);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int) WM.CLIPBOARDUPDATE)
                    ClipboardUpdated?.Invoke(this, EventArgs.Empty);

                base.WndProc(ref m);
            }

            protected override void OnClosing(CancelEventArgs e)
            {
                base.OnClosing(e);
                NativeMethods.RemoveClipboardFormatListener(Handle);
            }
        }
    }
}