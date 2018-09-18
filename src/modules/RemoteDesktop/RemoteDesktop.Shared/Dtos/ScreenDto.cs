using System.Drawing;

namespace RemoteDesktop.Shared.Dtos
{
    public class ScreenDto
    {
        public bool IsPrimary { get; set; }
        public Rectangle Bounds { get; set; }
    }
}