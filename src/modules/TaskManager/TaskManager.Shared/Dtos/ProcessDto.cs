using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TaskManager.Shared.Dtos
{
    public class ProcessDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public long WorkingSet { get; set; }
        public long PrivateBytes { get; set; }
        public byte[] IconData { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public ProcessPriorityClass PriorityClass { get; set; }
        public bool CanChangePriorityClass { get; set; }
        public int ParentProcess { get; set; }
        public string ProcessOwner { get; set; }
        public ProcessStatus Status { get; set; }
        public string FileName { get; set; }
        public string CommandLine { get; set; }
        public string ProductVersion { get; set; }
        public string FileVersion { get; set; }
        public long? MainWindowHandle { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }

    public class ChangeSet<T>
    {
        public T Value { get; set; }
        public EntryAction Action { get; set; }
    }

    public enum EntryAction
    {
        Add,
        Remove
    }

    public enum ProcessStatus
    {
        None,
        UserProcess,
        NetAssembly,
        Service,
        Immersive
    }
}