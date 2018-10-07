using System;

namespace Orcus.Administration.Library.Tasks
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskAudienceAttribute : Attribute
    {
        public TaskAudienceAttribute(TaskAudienceMode mode)
        {
            Mode = mode;
        }

        public TaskAudienceMode Mode { get; }
    }
}