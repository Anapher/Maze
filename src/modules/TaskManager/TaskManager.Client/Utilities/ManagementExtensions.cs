using System;
using System.Linq;
using System.Management;

namespace TaskManager.Client.Utilities
{
    public static class ManagementExtensions
    {
        public static bool TryGetProperty<T>(this ManagementObject managementObject, string propertyName, out T value)
        {
            var property = managementObject.Properties.Cast<PropertyData>().FirstOrDefault(x => x.Name == propertyName);
            if (property != null && property.Value is T val)
            {
                value = val;
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryGetDateTime(this ManagementObject managementObject, string propertyName, out DateTimeOffset value)
        {
            if (TryGetProperty(managementObject, propertyName, out string dmtfDate))
            {
                try
                {
                    value = ManagementDateTimeConverter.ToDateTime(dmtfDate);
                    return true;
                }
                catch (Exception e)
                {
                    // ignored
                }
            }

            value = default;
            return false;
        }
    }
}