namespace Orcus.Client.Library.Services
{
    public interface ICoreConnector
    {
        IServerConnection CurrentConnection { get; }
    }
}
