using System;

namespace DeviceManager.Shared.Dtos
{
    public class DeviceCategoryDisplayNameAttribute : Attribute
    {
        public DeviceCategoryDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; set; }
    }
}