using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Client.Native;
using x264net;

namespace RemoteDesktop.Client.Encoder.x264
{
    // ReSharper disable once InconsistentNaming
    public class x264StreamEncoder : IStreamEncoder
    {
        private X264Net _x264Net;
        private IFrameTransmitter _transmitter;

        public string Id { get; } = "x264";
        public bool IsPlatformSupported { get; } = true;

        public void Initialize(ScreenInfo screenInfo, IFrameTransmitter transmitter, Dictionary<string, string> options)
        {
            if (_x264Net != null)
                throw new InvalidOperationException("Already initialized.");

            _x264Net = new X264Net(screenInfo.Width, screenInfo.Height);
            _transmitter = transmitter;
        }

        public void Dispose()
        {
            _x264Net?.Dispose();
        }

        public unsafe void CodeImage(IntPtr scan0, PixelFormat pixelFormat)
        {
            var scan0Pointer = scan0.ToPointer();
            var result = _x264Net.EncodeFrame(scan0Pointer).Cast<X264Nal>().ToArray();

            if (!result.Any())
                return;

            var length = result.Sum(x => x.Length);
            var buffer = _transmitter.AllocateBuffer(length);
            try
            {
                var position = 0;
                fixed (byte* dataPointer = buffer.Array)
                {
                    foreach (var x264Nal in result)
                    {
                        NativeMethods.memcpy(dataPointer + buffer.Offset + position, x264Nal.Pointer, (UIntPtr) x264Nal.Length);
                        position += x264Nal.Length;
                    }
                }

                _transmitter.SendFrame(buffer);
            }
            finally
            {
                _transmitter.ReleaseSendBuffer(buffer);
            }
        }
    }
}