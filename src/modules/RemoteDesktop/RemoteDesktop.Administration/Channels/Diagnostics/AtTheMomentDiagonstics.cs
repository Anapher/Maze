using System;
using System.Threading;

namespace RemoteDesktop.Administration.Channels.Diagnostics
{
    public class AtTheMomentDiagonstics : IRemoteDesktopDiagonstics
    {
        private long _dataReceived;
        private int _framesProcessed;
        private DateTimeOffset _lastUpdateTimestamp;

        public void StartRecording()
        {
            _lastUpdateTimestamp = DateTimeOffset.UtcNow;
        }

        public void ReceivedData(int length)
        {
            Interlocked.Add(ref _dataReceived, length);
        }

        public void ProcessedFrame()
        {
            _framesProcessed++;

            CheckForUpdate();
        }

        public event EventHandler<DiagnosticData> UpdateDiagnostics;

        private void CheckForUpdate()
        {
            var diff = DateTimeOffset.UtcNow - _lastUpdateTimestamp;
            if (diff.TotalSeconds >= 1)
            {
                _lastUpdateTimestamp = DateTimeOffset.UtcNow;
                var dataReceived = Interlocked.Exchange(ref _dataReceived, 0);
                var framesProcessed = _framesProcessed;
                _framesProcessed = 0;

                var fps = (int) Math.Round(framesProcessed / diff.TotalSeconds);
                var bytesPerSecond = (int) (dataReceived / diff.TotalSeconds);

                UpdateDiagnostics?.Invoke(this, new DiagnosticData {Fps = fps, BytesPerSecond = bytesPerSecond});
            }
        }
    }
}