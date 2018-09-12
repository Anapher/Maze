using System;
using System.Diagnostics;
using System.Management;
using TaskManager.Shared.Dtos;

namespace TaskManager.Client.Utilities
{
    public static class ProcessPropertiesDtoExtensions
    {
        public static void ApplyProperties(this ChangingProcessPropertiesDto dto, Process process, ManagementObject wmiProcess)
        {
            if (process.HasExited)
            {
                dto.Status = ProcessStatus.Exited;
                return;
            }

            try
            {
                dto.PrivateBytes = process.PrivateMemorySize64;
                dto.WorkingSetSize = process.WorkingSet64;
            }
            catch (Exception)
            {
                // ignored
            }

            dto.HandleCount = process.HandleCount;
            dto.TotalProcessorTime = process.TotalProcessorTime;
            dto.UserProcessorTime = process.UserProcessorTime;
            dto.Status = process.Responding ? ProcessStatus.Running : ProcessStatus.NotResponding;

            if (wmiProcess.TryGetProperty("KernelModeTime", out ulong kernelModeTime))
                dto.KernelModeTime = TimeSpan.FromTicks((long) kernelModeTime);

            if (wmiProcess.TryGetProperty("UserModeTime", out ulong userModeTime))
                dto.UserModeTime = TimeSpan.FromTicks((long) userModeTime);

            if (wmiProcess.TryGetProperty("Priority", out uint priority))
                dto.Priority = priority;

            if (wmiProcess.TryGetProperty("PageFaults", out uint pageFaults))
                dto.PageFaults = pageFaults;

            if (wmiProcess.TryGetProperty("OtherOperationCount", out ulong otherOperationCount))
                dto.OtherOperationCount = otherOperationCount;

            if (wmiProcess.TryGetProperty("OtherTransferCount", out ulong otherTransferCount))
                dto.OtherTransferCount = otherTransferCount;

            if (wmiProcess.TryGetProperty("PeakPageFileUsage", out uint peakPageFileUsage))
                dto.PeakPageFileUsage = peakPageFileUsage;

            if (wmiProcess.TryGetProperty("PeakVirtualSize", out ulong peakVirtualSize))
                dto.PeakVirtualSize = peakVirtualSize;

            if (wmiProcess.TryGetProperty("PeakWorkingSetSize", out ulong peakWorkingSetSize))
                dto.PeakWorkingSetSize = peakWorkingSetSize;

            if (wmiProcess.TryGetProperty("PrivatePageCount", out ulong privatePageCount))
                dto.PrivatePageCount = privatePageCount;

            if (wmiProcess.TryGetProperty("PageFileUsage", out uint pageFileUsage))
                dto.PageFileUsage = pageFileUsage;

            if (wmiProcess.TryGetProperty("QuotaNonPagedPoolUsage", out uint quotaNonPagedPoolUsage))
                dto.QuotaNonPagedPoolUsage = quotaNonPagedPoolUsage;

            if (wmiProcess.TryGetProperty("QuotaPagedPoolUsage", out uint quotaPagedPoolUsage))
                dto.QuotaPagedPoolUsage = quotaPagedPoolUsage;

            if (wmiProcess.TryGetProperty("QuotaPeakNonPagedPoolUsage", out uint quotaPeakNonPagedPoolUsage))
                dto.QuotaPeakNonPagedPoolUsage = quotaPeakNonPagedPoolUsage;

            if (wmiProcess.TryGetProperty("QuotaPeakPagedPoolUsage", out uint quotaPeakPagedPoolUsage))
                dto.QuotaPeakPagedPoolUsage = quotaPeakPagedPoolUsage;

            if (wmiProcess.TryGetProperty("ReadOperationCount", out ulong readOperationCount))
                dto.ReadOperationCount = readOperationCount;

            if (wmiProcess.TryGetProperty("ReadTransferCount", out ulong readTransferCount))
                dto.ReadTransferCount = readTransferCount;

            if (wmiProcess.TryGetProperty("ThreadCount", out uint threadCount))
                dto.ThreadCount = threadCount;

            if (wmiProcess.TryGetProperty("VirtualSize", out ulong virtualSize))
                dto.VirtualSize = virtualSize;

            if (wmiProcess.TryGetProperty("WriteOperationCount", out ulong writeOperationCount))
                dto.WriteOperationCount = writeOperationCount;

            if (wmiProcess.TryGetProperty("WriteTransferCount", out ulong writeTransferCount))
                dto.WriteTransferCount = writeTransferCount;
        }
    }
}