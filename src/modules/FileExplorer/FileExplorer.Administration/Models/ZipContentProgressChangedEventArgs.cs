using System;

namespace FileExplorer.Administration.Models
{
    public class ZipContentProgressChangedEventArgs : EventArgs
    {
        public ZipContentProgressChangedEventArgs(double progress, long totalSize, long processedSize, double speed, TimeSpan estimatedTime)
        {
            Progress = progress;
            TotalSize = totalSize;
            ProcessedSize = processedSize;
            Speed = speed;
            EstimatedTime = estimatedTime;
        }

        public double Progress { get; }
        public long TotalSize { get; }
        public long ProcessedSize { get; }
        public double Speed { get; }
        public TimeSpan EstimatedTime { get;}
    }
}