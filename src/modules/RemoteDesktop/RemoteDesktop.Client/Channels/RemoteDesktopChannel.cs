using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Maze.Modules.Api;
using Maze.Modules.Api.Routing;
using Maze.Utilities;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Client.Encoder;
using RemoteDesktop.Shared;

namespace RemoteDesktop.Client.Channels
{
    [Route("screen"), SynchronizedChannel]
    public class RemoteDesktopChannel : MazeChannel, IFrameTransmitter
    {
        private bool _isDisposed;
        private bool _isCapturing;
        private readonly object _captureLock = new object();

        private readonly IEnumerable<IScreenCaptureService> _captureServices;
        private readonly IEnumerable<IStreamEncoder> _streamEncoders;

        private IStreamEncoder _streamEncoder;
        private IScreenCaptureService _captureService;

        public RemoteDesktopChannel(IEnumerable<IScreenCaptureService> captureServices, IEnumerable<IStreamEncoder> streamEncoder)
        {
            _captureServices = captureServices;
            _streamEncoders = streamEncoder;
        }

        public override void ReceiveData(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        [MazeGet("start")]
        public IActionResult InitializeRemoteDesktop()
        {
            Task.Run(CaptureLoop).Forget(); //not long running
            return Ok();
        }

        public void CaptureLoop()
        {
            lock (_captureLock)
            {
                if (_isCapturing)
                    throw new InvalidOperationException("The channel only allows one capture process");

                _isCapturing = true;
            }

            using (var captureService = _captureService)
            using (var streamEncoder = _streamEncoder)
            {
                while (!_isDisposed)
                {
                    captureService.Capture(streamEncoder);
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            var captureCommandLine = MazeContext.Request.Headers["capture"];
            var encoderCommandLine = MazeContext.Request.Headers[HeaderNames.AcceptEncoding];

            var captureOptions = ComponentOptions.Parse(captureCommandLine);
            var encoderOptions = ComponentOptions.Parse(encoderCommandLine);

            _captureService = ResolveService(captureOptions, _captureServices);
            _streamEncoder = ResolveService(encoderOptions, _streamEncoders);

            var screenInfo = _captureService.Initialize(captureOptions);
            _streamEncoder.Initialize(screenInfo, this, encoderOptions);
        }

        private static T ResolveService<T>(ComponentOptions options, IEnumerable<T> services) where T : IScreenComponent
        {
            var service = services.FirstOrDefault(x => x.Id == options.ComponentName);
            if (service?.IsPlatformSupported != true)
            {
                service = services.FirstOrDefault(x => x.IsPlatformSupported);
                if (service == null)
                    throw new InvalidOperationException($"No available service found for {typeof(T).Name}");
            }

            return service;
        }

        public override void Dispose()
        {
            base.Dispose();
            _isDisposed = true;

            lock (_captureLock)
            {
                if (!_isCapturing)
                {
                    _captureService?.Dispose();
                    _streamEncoder?.Dispose();
                    _isCapturing = true; //block start
                }
            }
        }

        public ArraySegment<byte> AllocateBuffer(int length)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(length + RequiredOffset);
            return new ArraySegment<byte>(buffer, RequiredOffset, length);
        }

        public void ReleaseSendBuffer(ArraySegment<byte> buffer)
        {
            ArrayPool<byte>.Shared.Return(buffer.Array);
        }

        public void SendFrame(ArraySegment<byte> sendBuffer)
        {
            Send(sendBuffer.Array, sendBuffer.Offset, sendBuffer.Count, hasOffset: true).Wait();
        }
    }
}