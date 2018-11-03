using System;

namespace Tasks.Infrastructure.Administration.PropertyGrid.Attributes
{
    /// <summary>
    ///     The boolean value will be represented by a checkbox instead of a combobox (default)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckBoxBooleanAttribute : Attribute
    {
    }
}