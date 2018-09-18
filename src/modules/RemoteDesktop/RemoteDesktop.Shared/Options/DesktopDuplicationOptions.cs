namespace RemoteDesktop.Shared.Options
{
    public class DesktopDuplicationOptions : ComponentOptions
    {
        public DesktopDuplicationOptions() : base("desktopduplication")
        {
            Monitor = new OptionKey<int>("monitor", this);
        }

        public OptionKey<int> Monitor { get; }
    }
}