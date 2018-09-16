using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Client.Encoder.x264;

namespace RemoteDesktop.Client.Encoder
{
    public interface IStreamEncoder : IScreenComponent, IDisposable
    {
        void Initialize(ScreenInfo screenInfo, IFrameTransmitter transmitter, Dictionary<string, string> options);
        void CodeImage(IntPtr scan0, PixelFormat pixelFormat);
    }
}