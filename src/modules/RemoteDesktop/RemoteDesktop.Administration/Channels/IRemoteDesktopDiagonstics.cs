namespace RemoteDesktop.Administration.Channels
{
    public interface IRemoteDesktopDiagonstics
    {
        void StartRecording();
        void ReceivedData(int length);
        void ProcessedFrame();
    }
}