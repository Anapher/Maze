using System;
using System.Drawing.Imaging;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Shared;

namespace RemoteDesktop.Client.Encoder
{
    public interface IStreamEncoder : IScreenComponent, IDisposable
    {
        void Initialize(ScreenInfo screenInfo, IFrameTransmitter transmitter, ComponentOptions componentOptions);
        void CodeImage(IntPtr scan0, PixelFormat pixelFormat);
    }
}