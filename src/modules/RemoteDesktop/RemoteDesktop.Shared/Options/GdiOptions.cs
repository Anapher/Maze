namespace RemoteDesktop.Shared.Options
{
    public class GdiOptions : ComponentOptions
    {
        public GdiOptions() : base("gdi+")
        {
            Monitor = new OptionKey<int>("monitor", this);
        }

        public OptionKey<int> Monitor { get; }
    }
}