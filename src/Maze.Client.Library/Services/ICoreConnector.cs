namespace Orcus.Client.Library.Services
{
    /// <summary>
    ///     The connector that will keep the connection to the server
    /// </summary>
    public interface ICoreConnector
    {
        /// <summary>
        ///     The current connection to the server. This value is <code>null</code> if the client is currently not connected.
        /// </summary>
        IServerConnection CurrentConnection { get; }
    }
}
