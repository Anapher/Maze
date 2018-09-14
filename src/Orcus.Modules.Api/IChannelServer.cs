using System;

namespace Orcus.Modules.Api
{
    public interface IChannelServer
    {
        int RegisterChannel(IDataChannel channel);
        IDataChannel GetChannel(int id);
    }

    public class SynchronizedChannelAttribute : Attribute
    {
    }
}