namespace RemoteDesktop.Client.Capture
{
    public struct ScreenInfo
    {
        public ScreenInfo(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}