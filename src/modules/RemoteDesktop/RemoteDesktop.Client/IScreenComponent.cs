namespace RemoteDesktop.Client
{
    public interface IScreenComponent
    {
        string Id { get; }
        bool IsPlatformSupported { get; }
    }
}