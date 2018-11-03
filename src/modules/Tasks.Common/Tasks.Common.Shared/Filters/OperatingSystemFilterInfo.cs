using Tasks.Infrastructure.Core.Filter;

namespace Tasks.Common.Server.Filters
{
    public class OperatingSystemFilterInfo : FilterInfo
    {
        public bool Windows7 { get; set; }
        public bool Windows81 { get; set; }
        public bool Windows10 { get; set; }
        public bool WindowsServer2008 { get; set; }
        public bool WindowsServer2012 { get; set; }
        public bool WindowsServer2016 { get; set; }
    }
}