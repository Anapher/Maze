using System;
using System.Collections.Generic;
using RemoteDesktop.Client.Encoder;

namespace RemoteDesktop.Client.Capture
{
    public interface IScreenCaptureService : IScreenComponent, IDisposable
    {
        ScreenInfo Initialize(Dictionary<string, string> options);

        void Capture(IStreamEncoder encoder);
    }
}