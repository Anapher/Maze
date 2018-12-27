namespace Orcus.Administration.Library.Clients
{
    /// <summary>
    ///     A rest client that sends all messages to a specific client. All channels created over this client will be disposed with the object.
    /// </summary>
    public interface ITargetedRestClient : IOrcusRestClient
    {
    }
}