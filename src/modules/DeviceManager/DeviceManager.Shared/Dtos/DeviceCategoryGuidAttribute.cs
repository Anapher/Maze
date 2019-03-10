using System;

namespace DeviceManager.Shared.Dtos
{
    public class DeviceCategoryGuidAttribute : Attribute
    {
        public DeviceCategoryGuidAttribute(Guid guid)
        {
            Guid = guid;
        }

        public DeviceCategoryGuidAttribute(string guid)
        {
            Guid = new Guid(guid);
        }

        public Guid Guid { get; }
    }
}