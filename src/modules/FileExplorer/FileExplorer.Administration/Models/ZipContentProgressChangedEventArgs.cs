using System;

namespace FileExplorer.Administration.Models
{
    public class ZipContentProgressChangedEventArgs : EventArgs
    {
        public double Progress { get; set; }
        public long TotalSize { get; set; }
        public long ProcessedSize { get; set; }
        public double Speed { get; set; }
        public TimeSpan EstimatedTime { get; set; }
    }
}