namespace RequestTransmitter.Client.Options
{
    public class RequestTransmitterOptions
    {
        public string RequestDirectory { get; set; } = "%appdata%/Maze/RequestTransmitter";
        public int RequestTimeoutSeconds { get; set; } = 4;
    }
}