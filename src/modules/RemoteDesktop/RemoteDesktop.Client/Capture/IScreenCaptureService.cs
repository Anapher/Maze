using System;
using System.Collections.Generic;
using RemoteDesktop.Client.Encoder;
using RemoteDesktop.Shared;

namespace RemoteDesktop.Client.Capture
{
    public interface IScreenCaptureService : IScreenComponent, IDisposable
    {
        ScreenInfo Initialize(ComponentOptions componentOptions);

        void Capture(IStreamEncoder encoder);
    }
}