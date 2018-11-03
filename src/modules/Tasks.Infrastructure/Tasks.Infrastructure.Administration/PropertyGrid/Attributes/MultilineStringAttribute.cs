using System;

namespace Tasks.Infrastructure.Administration.PropertyGrid.Attributes
{
    /// <summary>
    ///     The string can be multiline, a bigger text field will appear and returns will be accepted
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MultilineStringAttribute : Attribute
    {
    }
}