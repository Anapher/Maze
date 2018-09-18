namespace RemoteDesktop.Shared.Dtos
{
    public class ParametersDto
    {
        public string[] Encoders { get; set; }
        public string[] CaptureServices { get; set; }
        public ScreenDto[] Screens { get; set; }
    }
}