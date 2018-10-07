using System;

namespace Orcus.Administration.Library.Tasks
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskStopRequiredAttribute : Attribute
    {
    }
}