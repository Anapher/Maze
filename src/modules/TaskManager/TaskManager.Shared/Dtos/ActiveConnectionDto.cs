namespace TaskManager.Shared.Dtos
{
    public class ActiveConnectionDto
    {
        public ProtocolName ProtocolName { get; set; }
        public string LocalAddress { get; set; }
        public string RemoteAddress { get; set; }
        public int LocalPort { get; set; }
        public int RemotePort { get; set; }
        public ConnectionState State { get; set; }
    }

    public enum ProtocolName
    {
        Tcp,
        Udp
    }

    public enum ConnectionState
    {
        NoError,
        Established,
        Listening,
        TimeWait,
        CloseWait,
        Other
    }
}