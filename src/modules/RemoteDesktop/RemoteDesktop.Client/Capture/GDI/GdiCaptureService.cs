using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using RemoteDesktop.Client.Encoder;
using RemoteDesktop.Client.Extensions;
using RemoteDesktop.Shared;
using RemoteDesktop.Shared.Options;

namespace RemoteDesktop.Client.Capture.GDI
{
    public class GdiCaptureService : IScreenCaptureService
    {
        // ReSharper disable once InconsistentNaming
        private const int SRCCOPY = 0x00CC0020;

        private Bitmap _currentImage;
        private Rectangle _screenBounds;
        private IntPtr _screenDeviceContext;

        public string Id { get; } = "gdi+";
        public bool IsPlatformSupported { get; } = true; //GDI is supported everywhere

        public ScreenInfo Initialize(ComponentOptions componentOptions)
        {
            var options = componentOptions.To<GdiOptions>();
            var screen = Screen.AllScreens[options.Monitor.Value];

            _screenBounds = screen.Bounds;

            _currentImage = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format24bppRgb);
            _screenDeviceContext = NativeMethods.CreateDC("DISPLAY", null, null, IntPtr.Zero);

            if (_screenDeviceContext == IntPtr.Zero)
                throw new InvalidOperationException("Creating device context for display failed.");

            return new ScreenInfo(screen.Bounds.Width, screen.Bounds.Height);
        }

        public void Dispose()
        {
            _currentImage?.Dispose();
            if (_screenDeviceContext != IntPtr.Zero)
                NativeMethods.DeleteDC(_screenDeviceContext);

            _currentImage = null;
            _screenDeviceContext = IntPtr.Zero;
        }

        public void Capture(IStreamEncoder encoder)
        {
            using (var graphics = Graphics.FromImage(_currentImage))
            {
                var deviceContext = graphics.GetHdc();
                NativeMethods.BitBlt(deviceContext, 0, 0, _screenBounds.Width, _screenBounds.Height, _screenDeviceContext, _screenBounds.X,
                    _screenBounds.Y, SRCCOPY);
                graphics.ReleaseHdc(deviceContext);

                var bitmapData = _currentImage.LockBits(new Rectangle(0, 0, _currentImage.Width, _currentImage.Height), ImageLockMode.ReadOnly,
                    _currentImage.PixelFormat);

                try
                {
                    encoder.CodeImage(bitmapData.Scan0, bitmapData.PixelFormat);
                }
                finally
                {
                    _currentImage.UnlockBits(bitmapData);
                }
            }
        }
    }
}