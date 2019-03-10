using System;
using System.Reflection;
using DeviceManager.Shared.Dtos;

namespace DeviceManager.Shared.Utilities
{
    public static class DeviceCategoryExtensions
    {
        public static Guid GetGuid(this DeviceCategory deviceCategory)
        {
            var type = typeof(DeviceCategory);
            return type.GetMember(deviceCategory.ToString())[0].GetCustomAttribute<DeviceCategoryGuidAttribute>().Guid;
        }

        public static string GetDisplayName(this DeviceCategory deviceCategory)
        {
            var type = typeof(DeviceCategory);
            return type.GetMember(deviceCategory.ToString())[0].GetCustomAttribute<DeviceCategoryDisplayNameAttribute>().DisplayName;
        }
    }
}