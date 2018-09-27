using System;
using System.Collections.Generic;
using Microsoft.Win32;
using Orcus.Modules.Api;
using Orcus.Modules.Api.Parameters;
using Orcus.Modules.Api.Routing;
using RegistryEditor.Client.Utilities;
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
        public IActionResult GetSubKeyValues([FromQuery] string path)
        {
            var (registryHive, relativePath) = RegistryHelper.UnpackPath(path);

            using (var regKey = RegistryHelper.OpenRegistry(registryHive).OpenSubKey(relativePath ?? string.Empty, RegistryKeyPermissionCheck.ReadSubTree))
            {
                var result = new List<RegistryValueDto>();
                foreach (var valueName in regKey.GetValueNames())
                {
                    var kind = regKey.GetValueKind(valueName);
                    switch (kind)
                    {
                        case RegistryValueKind.String:
                            result.Add(new StringRegistryValueDto {Name = valueName, Value = (string) regKey.GetValue(valueName, string.Empty)});
                            break;
                        case RegistryValueKind.ExpandString:
                            result.Add(new ExpandableStringRegistryValueDto
                            {
                                Name = valueName, Value = (string) regKey.GetValue(valueName, string.Empty)
                            });
                            break;
                        case RegistryValueKind.Binary:
                            result.Add(new BinaryRegistryValueDto {Name = valueName, Value = (byte[]) regKey.GetValue(valueName, new byte[0])});
                            break;
                        case RegistryValueKind.DWord:
                            result.Add(new DWordRegistryValueDto {Name = valueName, Value = (uint) (int) regKey.GetValue(valueName, 0)});
                            break;
                        case RegistryValueKind.MultiString:
                            result.Add(new MultiStringRegistryValueDto
                            {
                                Name = valueName, Value = (string[]) regKey.GetValue(valueName, new string[0])
                            });
                            break;
                        case RegistryValueKind.QWord:
                            result.Add(new QWordRegistryValueDto {Name = valueName, Value = (ulong) (long) regKey.GetValue(valueName, 0L)});
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return Ok(result);
            }
        }

        [OrcusDelete("subKeys/values")]
        public IActionResult DeleteSubKeyValue([FromQuery] string path, [FromQuery] string name)
        {
            var (registryHive, relativePath) = RegistryHelper.UnpackPath(path);

            using (var regKey = RegistryHelper.OpenRegistry(registryHive).OpenSubKey(relativePath ?? string.Empty, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                regKey.DeleteValue(name, throwOnMissingValue: true);
                return Ok();
            }
        }

        [OrcusPost("subKeys/values")]
        public IActionResult CreateSubKeyValue([FromQuery] string path, [FromBody] RegistryValueDto valueDto)
        {
            var (registryHive, relativePath) = RegistryHelper.UnpackPath(path);

            using (var regKey = RegistryHelper.OpenRegistry(registryHive).OpenSubKey(relativePath ?? string.Empty, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                object value;
                switch (valueDto)
                {
                    case var val when val is StringRegistryValueDto typedVal:
                        value = typedVal.Value;
                        break;
                    case var val when val is ExpandableStringRegistryValueDto typedVal:
                        value = typedVal.Value;
                        break;
                    case var val when val is BinaryRegistryValueDto typedVal:
                        value = typedVal.Value;
                        break;
                    case var val when val is DWordRegistryValueDto typedVal:
                        value = (int) typedVal.Value;
                        break;
                    case var val when val is QWordRegistryValueDto typedVal:
                        value = (long)typedVal.Value;
                        break;
                    case var val when val is MultiStringRegistryValueDto typedVal:
                        value = typedVal.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                regKey.SetValue(valueDto.Name, value);
                return Ok();
            }
        }
    }
}