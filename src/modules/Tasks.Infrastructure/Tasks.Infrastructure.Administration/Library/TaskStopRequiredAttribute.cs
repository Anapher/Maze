using System;

namespace Tasks.Infrastructure.Administration.Library
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskStopRequiredAttribute : Attribute
    {
    }
}