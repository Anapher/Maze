using System;

namespace TaskManager.Shared.Dtos
{
    public class ChangingProcessPropertiesDto
    {
        public int HandleCount { get; set; }
        public TimeSpan KernelModeTime { get; set; }
        public TimeSpan UserModeTime { get; set; }
        public uint Priority { get; set; }
        public uint PageFaults { get; set; }
        public ulong PeakWorkingSetSize { get; set; }
        public ulong ReadOperationCount { get; set; }
        public ulong ReadTransferCount { get; set; }
        public ulong WriteOperationCount { get; set; }
        public ulong WriteTransferCount { get; set; }
        public ulong OtherOperationCount { get; set; }
        public ulong OtherTransferCount { get; set; }
        public uint PageFileUsage { get; set; }
        public uint PeakPageFileUsage { get; set; }
        public ulong PeakVirtualSize { get; set; }
        public ulong VirtualSize { get; set; }
        public uint QuotaNonPagedPoolUsage { get; set; }
        public uint QuotaPagedPoolUsage { get; set; }
        public uint QuotaPeakNonPagedPoolUsage { get; set; }
        public uint QuotaPeakPagedPoolUsage { get; set; }
        public ulong PrivatePageCount { get; set; }
        public uint ThreadCount { get; set; }
        public long PrivateBytes { get; set; }
        public long WorkingSetSize { get; set; }

        public TimeSpan TotalProcessorTime { get; set; }
        public TimeSpan UserProcessorTime { get; set; }

        public ProcessStatus Status { get; set; }
    }

    public class ProcessPropertiesDto : ChangingProcessPropertiesDto
    {
        public byte[] Icon { get; set; }
        public bool? IsWow64Process { get; set; }
    }

    public enum ProcessStatus
    {
        Running,
        NotResponding,
        Exited
    }
}