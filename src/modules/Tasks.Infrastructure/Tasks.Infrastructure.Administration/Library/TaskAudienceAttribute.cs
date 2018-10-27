using System;

namespace Tasks.Infrastructure.Administration.Library
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