using System;
using System.Threading.Tasks;
using ClipboardManager.Shared.Native;
using System.Windows.Forms;

#if WPF
using Maze.Administration.Library.Services;
using IDataObject = System.Windows.IDataObject;
using Clipboard = System.Windows.Clipboard;
#else
using Maze.Client.Library.Services;
#endif

namespace ClipboardManager.Shared.Utilities
{
    /// <summary>
    ///     Provides notifications when the contents of the clipboard is updated. This class is thread safe.
    /// </summary>
    public class ClipboardWatcher
    {
        private static readonly object EventLock = new object();
        private EventHandler<IDataObject> _clipboardChangeEventHandler;
        private NotificationWindow _form;

#if WPF
        private readonly IAppDispatcher _dispatcher;

        public ClipboardWatcher(IAppDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
#else
        private readonly IStaSynchronizationContext _synchronizationContext;

        public ClipboardWatcher(IStaSynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }
#endif

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
#if WPF
                        _dispatcher.Current.Invoke(new Action(() => _form = new NotificationWindow()));
#else
                        _synchronizationContext.Current.Send(state =>
                        {
                            _form = new NotificationWindow();
                        }, null);
#endif

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
                        _form?.DestroyHandle();
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
        private class NotificationWindow : NativeWindow
        {
            private static readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

            public NotificationWindow()
            {
                CreateHandle(new CreateParams());

                NativeMethods.SetParent(Handle, HWND_MESSAGE);
                NativeMethods.AddClipboardFormatListener(Handle);
            }

            public event EventHandler ClipboardUpdated;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int) WM.CLIPBOARDUPDATE)
                    ClipboardUpdated?.Invoke(this, EventArgs.Empty);

                base.WndProc(ref m);
            }

            public override void DestroyHandle()
            {
                NativeMethods.RemoveClipboardFormatListener(Handle);
                base.DestroyHandle();
            }
        }
    }
}