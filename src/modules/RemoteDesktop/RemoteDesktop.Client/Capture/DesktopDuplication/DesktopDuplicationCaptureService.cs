using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using RemoteDesktop.Client.Encoder;
using RemoteDesktop.Client.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;
using ResultCode = SharpDX.DXGI.ResultCode;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace RemoteDesktop.Client.Capture.DesktopDuplication
{
    public class DesktopDuplicationCaptureService : IScreenCaptureService
    {
        private Device _captureDevice;
        private OutputDescription _outputDesc;
        private Texture2DDescription _textureDesc;
        private OutputDuplication _deskDupl;
        private Texture2D _desktopImageTexture;
        private OutputDuplicateFrameInformation _frameInfo;

        public void Dispose()
        {
            _desktopImageTexture?.Dispose();
            _captureDevice?.Dispose();
            _deskDupl?.Dispose();
        }

        public string Id { get; } = "desktopduplication";

        public bool IsPlatformSupported
        {
            get
            {
                if (!PlatformHelper.IsWindows8OrNewer())
                    return false;

                try
                {
                    using (var factory = new Factory1())
                    using (var adapter = GetBestAdapter(factory))
                    using (new Device(adapter)) { }

                    return true;
                }
                catch (SharpDXException)
                {
                    return false;
                }
            }
        }

        public ScreenInfo Initialize(Dictionary<string, string> options)
        {
            var monitor = CaptureOptions.Monitor.GetValue(options);

            Adapter1 adapter;
            try
            {
                using (var factory = new Factory1())
                    adapter = GetBestAdapter(factory);
            }
            catch (SharpDXException e)
            {
                throw new InvalidOperationException("Could not find the specified graphics card adapter.", e);
            }

            Output output;
            using (adapter)
            {
                _captureDevice = new Device(adapter);

                try
                {
                    output = adapter.GetOutput(monitor);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Could not find the specified output device.", e);
                }
            }

            using (output)
            using (var output1 = output.QueryInterface<Output1>())
            {
                _outputDesc = output.Description;
                _textureDesc = new Texture2DDescription
                {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = GetWidth(_outputDesc.DesktopBounds),
                    Height = GetHeight(_outputDesc.DesktopBounds),
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                };

                try
                {
                    _deskDupl = output1.DuplicateOutput(_captureDevice);
                }
                catch (SharpDXException ex) when (ex.ResultCode.Code == ResultCode.NotCurrentlyAvailable.Result.Code)
                {
                    throw new InvalidOperationException(
                        "There is already the maximum number of applications using the Desktop Duplication API running, please close one of the applications and try again.");
                }

                return new ScreenInfo(_textureDesc.Width, _textureDesc.Height);
            }
        }

        public void Capture(IStreamEncoder encoder)
        {
            if (!RetrieveFrame())
                return;

            try
            {
                var mapSource = _captureDevice.ImmediateContext.MapSubresource(_desktopImageTexture, 0, MapMode.Read, MapFlags.None);
                try
                {
                    // Get the desktop capture texture
                    encoder.CodeImage(mapSource.DataPointer, PixelFormat.Format32bppArgb);
                }
                finally
                {
                    _captureDevice.ImmediateContext.UnmapSubresource(_desktopImageTexture, 0);
                }
            }
            finally
            {
                ReleaseFrame();
            }
        }

        private void ReleaseFrame()
        {
            try
            {
                _deskDupl.ReleaseFrame();
            }
            catch (SharpDXException ex) when (!ex.ResultCode.Failure)
            {
            }
        }

        private bool RetrieveFrame()
        {
            if (_desktopImageTexture == null)
                _desktopImageTexture = new Texture2D(_captureDevice, _textureDesc);
            SharpDX.DXGI.Resource desktopResource;
            _frameInfo = new OutputDuplicateFrameInformation();

            try
            {
                _deskDupl.AcquireNextFrame(500, out _frameInfo, out desktopResource);
            }
            catch (SharpDXException ex) when (ex.ResultCode.Code == ResultCode.WaitTimeout.Result.Code)
            {
                return false;
            }

            using (desktopResource)
            using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
            {
                _captureDevice.ImmediateContext.CopyResource(tempTexture, _desktopImageTexture);
            }

            return true;
        }

        private static Adapter1 GetBestAdapter(Factory1 factory1)
        {
            return factory1.Adapters1.FirstOrDefault(x => x.Outputs.Any()) ?? factory1.Adapters1.First();
        }

        public static int GetWidth(RawRectangle rawRectangle)
        {
            return rawRectangle.Right - rawRectangle.Left;
        }

        public static int GetHeight(RawRectangle rawRectangle)
        {
            return rawRectangle.Bottom - rawRectangle.Top;
        }
    }
}
