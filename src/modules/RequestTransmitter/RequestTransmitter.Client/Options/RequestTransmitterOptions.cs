namespace RequestTransmitter.Client.Options
{
    public class RequestTransmitterOptions
    {
        public string RequestDirectory { get; set; } = "%appdata%/Orcus/RequestTransmitter";
        public int RequestTimeoutSeconds { get; set; } = 4;
    }
}