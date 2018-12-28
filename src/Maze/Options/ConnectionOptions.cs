namespace Maze.Options
{
    public class ConnectionOptions
    {
        public string[] ServerUris { get; set; }
        public int ReconnectDelay { get; set; }
        public int KeepAliveInterval { get; set; }
    }
}