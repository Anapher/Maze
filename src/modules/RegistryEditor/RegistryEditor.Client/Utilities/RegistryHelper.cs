using System;
using System.Linq;
using Microsoft.Win32;

namespace RegistryEditor.Client.Utilities
{
    public static class RegistryHelper
    {
        public static RegistryKey OpenRegistry(RegistryHive registryHive)
        {
            switch (registryHive)
            {
                case RegistryHive.ClassesRoot:
                    return Registry.ClassesRoot;
                case RegistryHive.CurrentUser:
                    return Registry.CurrentUser;
                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine;
                case RegistryHive.Users:
                    return Registry.Users;
                case RegistryHive.CurrentConfig:
                    return Registry.CurrentConfig;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registryHive), registryHive, null);
            }
        }

        public static (RegistryHive, string) UnpackPath(string path)
        {
            var split = path.Split(new[] { '\\' }, 2);
            return (ConvertToRegistryHive(split[0]), split.Skip(1).FirstOrDefault());
        }

        public static RegistryHive ConvertToRegistryHive(string hiveName)
        {
            switch (hiveName)
            {
                case var name when name.Equals("HKEY_CLASSES_ROOT", StringComparison.OrdinalIgnoreCase):
                    return RegistryHive.ClassesRoot;
                case var name when name.Equals("HKEY_CURRENT_USER", StringComparison.OrdinalIgnoreCase):
                    return RegistryHive.CurrentUser;
                case var name when name.Equals("HKEY_LOCAL_MACHINE", StringComparison.OrdinalIgnoreCase):
                    return RegistryHive.LocalMachine;
                case var name when name.Equals("HKEY_USERS", StringComparison.OrdinalIgnoreCase):
                    return RegistryHive.Users;
                case var name when name.Equals("HKEY_CURRENT_CONFIG", StringComparison.OrdinalIgnoreCase):
                    return RegistryHive.CurrentConfig;
            }

            throw new ArgumentException("Invalid registry hive: " + hiveName, nameof(hiveName));
        }

        public static string ToPath(this RegistryHive hive)
        {
            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    return "HKEY_CLASSES_ROOT";
                case RegistryHive.CurrentUser:
                    return "HKEY_CURRENT_USER";
                case RegistryHive.LocalMachine:
                    return "HKEY_LOCAL_MACHINE";
                case RegistryHive.Users:
                    return "HKEY_USERS";
                case RegistryHive.CurrentConfig:
                    return "HKEY_CURRENT_CONFIG";
                default:
                    throw new ArgumentOutOfRangeException(nameof(hive), hive, null);
            }
        }
    }
}